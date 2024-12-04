using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.Models.DTOs;

namespace TarikhMaghribi.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetUserProfile(string userId);
        Task<IActionResult> UpdateUserProfile(string userId, UserProfileUpdateDto model);
        Task<IActionResult> ChangePassword(string userId, ChangePasswordDto request);
        Task<IActionResult> DeleteUserAccount(string userId, string password);
    }

}
