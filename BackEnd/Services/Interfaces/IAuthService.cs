using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.Models.DTOs.Auth;

namespace TarikhMaghribi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegistrationResult> RegisterNewUser(RegistreDto user);
        Task<IActionResult> LogIn(LoginDto login);
        Task<IActionResult> ConfirmEmail(Activate dto);
        Task<IActionResult> RequestPasswordReset(ResetPasswordRequest req);
        Task<IActionResult> ResetPassword(ResetPassword request);
    }
}
