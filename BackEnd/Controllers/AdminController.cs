using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.DBContext;
using Microsoft.EntityFrameworkCore;
using TarikhMaghribi.Services.Interfaces;
using TarikhMaghribi.Models.DTOs;
using TarikhMaghribi.Models.Entities;

namespace TarikhMaghribi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="superutilisateur")]

    public class AdminController : ControllerBase
    {

        private readonly IJourFerieService _jourFerieService;
        private readonly IAdminService _adminService;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(IJourFerieService jourFerieService,UserManager<AppUser> userManager)
        {
            _jourFerieService = jourFerieService;
            _userManager = userManager;
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

            var result = await _jourFerieService.CreateJourFerie(jourFerieDto);
            if (result)
            {
                return Ok(new { message = "Jour férié créé avec succès" });
            }

            return StatusCode(500, "Erreur interne du serveur.");
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

            var result = await _jourFerieService.UpdateJourFerie(id, jourFerieDto);
            if (!result)
            {
                return NotFound(new { message = "Jour férié non trouvé ou erreur lors de la mise à jour" });
            }

            return Ok(new { message = "Jour férié mis à jour avec succès" });
        }
        [HttpDelete("delete-jour-ferie/{id}")]
        public async Task<IActionResult> DeleteJourFerie(int id)
        {
            if (!User.IsInRole("superutilisateur"))
            {
                return Unauthorized(new { message = "Vous n'avez pas les autorisations nécessaires pour effectuer cette action." });
            }

            var result = await _jourFerieService.DeleteJourFerie(id);
            if (!result)
            {
                return NotFound(new { message = "Jour férié non trouvé ou erreur lors de la suppression" });
            }

            return Ok(new { message = "Jour férié supprimé avec succès" });
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
                var joursFeries = await _jourFerieService.GetAllJoursFeries();
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
                var users = await _adminService.GetUsers(currentUserId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [HttpGet("details/{userId}")]
        public async Task<IActionResult> GetUserDetails(string userId)
        {
            try
            {
                var userDetails = await _adminService.GetUserDetails(userId);
                if (userDetails == null)
                {
                    return NotFound(new { message = "Utilisateur non trouvé" });
                }

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var result = await _adminService.DeleteUser(userId);
                if (result)
                {
                    return Ok(new { message = "Compte et tâches associés supprimés avec succès" });
                }

                return BadRequest(new { message = "Échec de la suppression du compte" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

    }
}
