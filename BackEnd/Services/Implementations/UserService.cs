using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.DBContext;
using TarikhMaghribi.Services.Interfaces;
using TarikhMaghribi.Models.DTOs;
using TarikhMaghribi.Models.Entities;

namespace TarikhMaghribi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public UserService(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<UserProfileDto> GetUserProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserProfileDto
            {
                Email = user.Email,
                Nom = user.Nom,
                Prenom = user.Prenom,
                Phone = user.PhoneNumber ?? "-",
                DateDeNaissance = user.DateDeNaissance,
                Roles = roles.ToList()
            };
        }

        public async Task<IActionResult> UpdateUserProfile(string userId, UserProfileUpdateDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new NotFoundObjectResult("User not found.");

            user.Nom = model.Nom;
            user.Prenom = model.Prenom;
            user.PhoneNumber = model.Phone;
            user.DateDeNaissance = model.DateDeNaissance;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new OkObjectResult("User profile updated successfully.");
            }
            return new BadRequestObjectResult(result.Errors);
        }

        public async Task<IActionResult> ChangePassword(string userId, ChangePasswordDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new UnauthorizedResult();

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                return new OkObjectResult("Password changed successfully.");
            }
            return new BadRequestObjectResult(result.Errors);
        }

        public async Task<IActionResult> DeleteUserAccount(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new NotFoundObjectResult("User not found.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid) return new BadRequestObjectResult("Incorrect password.");

            // Remove all tasks associated with the user
            var userTasks = _context.Tasks.Where(t => t.UserId == userId);
            _context.Tasks.RemoveRange(userTasks);

            try
            {
                await _context.SaveChangesAsync();

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return new OkObjectResult("Account and associated tasks deleted successfully.");
                }
                return new BadRequestObjectResult("Failed to delete account.");
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }
    }

}
