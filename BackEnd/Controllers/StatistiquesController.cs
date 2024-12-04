using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.DBContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TarikhMaghribi.Services.Interfaces;
using TarikhMaghribi.Models.DTOs;
using TarikhMaghribi.Models.Entities;

namespace TarikhMaghribi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class StatistiquesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;
        private readonly UserManager<AppUser> _userManager;

        public StatistiquesController(IUserService userService, ITaskService taskService,UserManager<AppUser> userManager)
        {
            _userService = userService;
            _taskService = taskService;
            _userManager = userManager;
        }
        [HttpGet("task-stats/{annee}/{mois}")]
        public async Task<IActionResult> GetTaskStatistics(int annee, int mois)
        {
            var userId = _userManager.GetUserId(User); // Récupère l'ID de l'utilisateur connecté

            if (userId == null)
            {
                return Unauthorized(new { message = "Utilisateur non connecté." });
            }

            return await _taskService.GetTaskStatistics(userId, annee, mois);
        }

        [HttpGet("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = _userManager.GetUserId(User);
            var userProfile = await _userService.GetUserProfile(userId);
            if (userProfile == null)
            {
                return NotFound("User not found.");
            }
            return Ok(userProfile);
        }
        [HttpPost("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileUpdateDto model)
        {
            var userId = _userManager.GetUserId(User);
            return await _userService.UpdateUserProfile(userId, model);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var userId = _userManager.GetUserId(User);
            return await _userService.ChangePassword(userId, request);
        }
        [HttpDelete("DeleteUserAccount")]
        public async Task<IActionResult> DeleteAccount([FromQuery] string password)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            return await _userService.DeleteUserAccount(userId, password);
        }



    }
}
