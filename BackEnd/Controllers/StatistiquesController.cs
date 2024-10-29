using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.DBContext.Models;
using TarikhMaghribi.DBContext;
using TarikhMaghribi.DTO;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TarikhMaghribi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class StatistiquesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration configuration;
        public StatistiquesController(UserManager<AppUser> userManager, AppDbContext context, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            this.configuration = configuration;
        }
        [HttpGet("task-stats/{annee}/{mois}")]
        public async Task<IActionResult> GetTaskStatistics(int annee, int mois)
        {
            var userId = _userManager.GetUserId(User); // Récupère l'ID de l'utilisateur connecté

            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            try
            {
                // Récupération des tâches de l'utilisateur connecté pour le mois et l'année spécifiés
                var tasksInMonth = await _context.Tasks
                    .Where(t => t.UserId == userId && t.Date.Year == annee && t.Date.Month == mois)
                    .ToListAsync();

                var totalTasks = tasksInMonth.Count;
                var notStartedTasks = tasksInMonth.Count(t => t.Status == "pas encore");
                var inProgressTasks = tasksInMonth.Count(t => t.Status == "encore");
                var completedTasks = tasksInMonth.Count(t => t.Status == "terminé");

                // Récupération du nombre total de jours fériés pour le mois et l'année spécifiés
                var holidayCountMonth = await _context.JoursFeries
                    .Where(h => h.DateJour.Year == annee && h.DateJour.Month == mois)
                    .CountAsync();
                var holidayCountTotalYear = await _context.JoursFeries
                    .Where(h => h.DateJour.Year == annee )
                    .CountAsync();
                var result = new
                {
                    TotalTasks = totalTasks,
                    NotStartedTasks = notStartedTasks,
                    InProgressTasks = inProgressTasks,
                    CompletedTasks = completedTasks,
                    holidayCountByMonth = holidayCountMonth,
                    holidayCountTotalByYear= holidayCountTotalYear
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [HttpGet("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            var roles = await _userManager.GetRolesAsync(user);

            var userProfile = new UserProfileDto
            {
                Email = user.Email,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Phone = user.PhoneNumber ?? "-",
                DateDeNaissance = user.DateDeNaissance,
                Roles = roles.ToList()
            };

            return Ok(userProfile);
        }
        [HttpPost("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileUpdateDto model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Nom = model.Nom;
            user.Prenom = model.Prenom;
            user.PhoneNumber = model.Phone;
            user.DateDeNaissance = model.DateDeNaissance;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("User profile updated successfully.");
            }

            return BadRequest(result.Errors);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password changed successfully.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        [HttpDelete("DeleteUserAccount")]
        public async Task<IActionResult> DeleteAccount([FromQuery] string password)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Vérifiez le mot de passe
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid)
            {
                return BadRequest(new { message = "Mot de passe incorrect" });
            }

            // Supprime toutes les tâches associées à l'utilisateur
            var userTasks = _context.Tasks.Where(t => t.UserId == userId);
            _context.Tasks.RemoveRange(userTasks);

            try
            {
                await _context.SaveChangesAsync();

                // Supprime l'utilisateur après avoir supprimé les tâches
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { message = "Compte et tâches associés supprimés avec succès" });
                }
                return BadRequest(new { message = "Échec de la suppression du compte" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }



    }
}
