using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.DBContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TarikhMaghribi.Services.Interfaces;
using TarikhMaghribi.Models.DTOs;
using TarikhMaghribi.Models.Entities;

namespace TarikhMaghribi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly UserManager<AppUser> _userManager;

        public TasksController(ITaskService taskService, UserManager<AppUser> userManager)
        {
            _taskService = taskService;
            _userManager = userManager;
        }

        [HttpGet("tasks/recent-to-old")]
        public async Task<IActionResult> GetUserTasksRecentToOld()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            try
            {
                var tasks = await _taskService.GetUserTasksRecentToOld(userId);
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

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            try
            {
                var result = await _taskService.CreateTask(userId, taskDto);
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
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            try
            {
                var tasks = await _taskService.GetTasksByMonthAndYear(userId, annee, mois);
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
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            try
            {
                var result = await _taskService.UpdateTask(id, userId, taskDto);
                if (!result)
                {
                    return NotFound(new { message = "Tâche non trouvée ou vous n'êtes pas autorisé à la modifier." });
                }
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
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            try
            {
                var result = await _taskService.DeleteTask(id, userId);
                if (!result)
                {
                    return NotFound(new { message = "Tâche non trouvée ou vous n'êtes pas autorisé à la supprimer." });
                }
                return Ok(new { message = "Tâche supprimée avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }

    }
}
