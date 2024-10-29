using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static TarikhMaghribi.Extentions.Mail;
using TarikhMaghribi.DBContext.Models;
using TarikhMaghribi.DBContext;
using Microsoft.EntityFrameworkCore;
using TarikhMaghribi.DTO;

namespace TarikhMaghribi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="superutilisateur")]

    public class AdminController : ControllerBase
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration configuration;
        private readonly ISenderEmail emailSender;
        public AdminController(UserManager<AppUser> userManager, AppDbContext context, IConfiguration configuration, RoleManager<IdentityRole> roleManager, ISenderEmail emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            this.configuration = configuration;
            this.emailSender = emailSender;
        }
        [HttpPost("create-jour-ferie")]
        public async Task<IActionResult> CreateJourFerie([FromBody] JourFerieDto jourFerieDto)
        {
            if (!User.IsInRole("superutilisateur"))
            {
                return Unauthorized(new { message = "Vous n'avez pas les autorisations nécessaires pour effectuer cette action." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jourFerie = new JourFerie
            {
                Nom = jourFerieDto.Nom,
                Details = jourFerieDto.Details,
                DateJour = jourFerieDto.DateJour,
                Categorie = jourFerieDto.Categorie
            };

            try
            {
                _context.JoursFeries.Add(jourFerie);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Jour férié créé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }
        [HttpPut("update-jour-ferie/{id}")]
        public async Task<IActionResult> UpdateJourFerie(int id, [FromBody] JourFerieDto jourFerieDto)
        {
            if (!User.IsInRole("superutilisateur"))
            {
                return Unauthorized(new { message = "Vous n'avez pas les autorisations nécessaires pour effectuer cette action." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jourFerie = await _context.JoursFeries.FindAsync(id);
            if (jourFerie == null)
            {
                return NotFound(new { message = "Jour férié non trouvé" });
            }

            jourFerie.Nom = jourFerieDto.Nom;
            jourFerie.Details = jourFerieDto.Details;
            jourFerie.DateJour = jourFerieDto.DateJour;
            jourFerie.Categorie = jourFerieDto.Categorie;

            try
            {
                _context.JoursFeries.Update(jourFerie);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Jour férié mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }
        [HttpDelete("delete-jour-ferie/{id}")]
        public async Task<IActionResult> DeleteJourFerie(int id)
        {
            if (!User.IsInRole("superutilisateur"))
            {
                return Unauthorized(new { message = "Vous n'avez pas les autorisations nécessaires pour effectuer cette action." });
            }

            var jourFerie = await _context.JoursFeries.FindAsync(id);
            if (jourFerie == null)
            {
                return NotFound(new { message = "Jour férié non trouvé" });
            }

            try
            {
                _context.JoursFeries.Remove(jourFerie);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Jour férié supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }
        [HttpGet("jours-feries")]
        public async Task<IActionResult> GetJoursFeries()
        {
            if (!User.IsInRole("superutilisateur"))
            {
                return Unauthorized(new { message = "Vous n'avez pas les autorisations nécessaires pour effectuer cette action." });
            }

            try
            {
                var joursFeries = await _context.JoursFeries
                    .OrderByDescending(jf => jf.DateJour)
                    .ToListAsync();

                return Ok(joursFeries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            if (!User.IsInRole("superutilisateur"))
            {
               
                return Unauthorized(new { message = "Vous n'avez pas les autorisations nécessaires pour effectuer cette action." });
            }
            var currentUserId = _userManager.GetUserId(User);
            try
            {
                var users = await _userManager.Users
                    .Where(user => user.Id != currentUserId)
                    .ToListAsync();

                var result = new List<object>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    result.Add(new
                    {
                        user.Id,
                        username=user.Prenom+" "+user.Nom,
                        user.Email,
                        user.Prenom,
                        user.Nom,
                        user.PhoneNumber,
                        Roles = roles,
                        AccountStatus = user.EmailConfirmed ? "Activated" : "Pending"
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("details/{userId}")]
        public async Task<IActionResult> GetUserDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Prenom,
                user.Nom,
                Roles = roles
            });
        }


        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
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
