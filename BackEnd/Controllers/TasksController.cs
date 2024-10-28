using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.DBContext.Models;
using TarikhMaghribi.DBContext;
using TarikhMaghribi.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TarikhMaghribi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TasksController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration configuration;
        public TasksController(UserManager<AppUser> userManager, AppDbContext context, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            this.configuration = configuration;
        }
        [HttpGet("tasks/recent-to-old")]
        public async Task<IActionResult> GetUserTasksRecentToOld()
        {
            var userId = _userManager.GetUserId(User);  // Récupère l'ID de l'utilisateur connecté

            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            try
            {
                var tasks = await _context.Tasks
                    .Where(t => t.UserId == userId)       
                    .OrderByDescending(t => t.Date)  // Trier du plus récent au plus ancien
                    .ToListAsync();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }


        [HttpPost("create-task")]
        public async Task<IActionResult> CreateTask([FromBody] TaskDto taskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User); // user connecté

            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            var task = new DBContext.Models.Task
            {
                Date = taskDto.Date,
                Title = taskDto.Title,
                Status = taskDto.Status,
                UserId = userId  
            };

            try
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Tâche créée avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [HttpGet("tasks/{annee}/{mois}")]
        public async Task<IActionResult> GetTasksByMonthAndYear(int annee, int mois)
        {
            var userId = _userManager.GetUserId(User);  // user connecté

            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            try
            {
                var tasks = await _context.Tasks
                    .Where(t => t.UserId == userId && t.Date.Year == annee && t.Date.Month == mois)  // Filtrer par ID utilisateur, année et mois
                    .OrderBy(t => t.Date)  // Trier du plus ancien au plus récent
                    .ToListAsync();

                if (tasks.Count == 0)
                {
                    return NotFound(new { message = "Aucune tâche trouvée pour la période sélectionnée." });
                }

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [HttpPut("update-task/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto taskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);  // Récupère l'ID de l'utilisateur connecté

            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(new { message = "Tâche non trouvée." });
            }

            // Vérifie si la tâche appartient à l'utilisateur connecté
            if (task.UserId != userId)
            {
                return Forbid();
            }

            // Mise à jour des propriétés de la tâche
            task.Date = taskDto.Date;
            task.Title = taskDto.Title;
            task.Status = taskDto.Status;

            try
            {
                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Tâche mise à jour avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

        [HttpDelete("delete-task/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = _userManager.GetUserId(User);  // userconnecté

            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound(new { message = "Tâche non trouvée." });
            }

            // Vérifie si la tâche appartient à l'utilisateur connecté
            if (task.UserId != userId)
            {
                return Unauthorized(new { message = "Vous n'êtes pas autorisé à supprimer cette tâche." });
            }

            try
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Tâche supprimée avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

    }
}
