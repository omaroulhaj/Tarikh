using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.Models.DTOs.Auth;
using TarikhMaghribi.Models.Entities;
using TarikhMaghribi.Services.Interfaces;
using static TarikhMaghribi.Services.Mail;

namespace TarikhMaghribi.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ISenderEmail _emailSender;

        public AuthService(UserManager<AppUser> userManager,ITokenService tokenService, IConfiguration configuration, ISenderEmail emailSender)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _emailSender = emailSender;
        }

        public async Task<RegistrationResult> RegisterNewUser(RegistreDto user)
        {
            // Vérifier si l'email est déjà utilisé
            if (await _userManager.FindByEmailAsync(user.email) != null)
            {
                return new RegistrationResult { IsSuccess = false, ErrorMessage = "Email is already in use." };
            }
            // Créer un nouvel utilisateur
            AppUser appUser = new AppUser
            {
                UserName = user.email,
                Email = user.email,
                Prenom = user.prenom,
                Nom = user.nom,
                PhoneNumber = user.phoneNumber,
                DateDeNaissance = user.dateDeNaissance,
            };

            var result = await _userManager.CreateAsync(appUser, user.password);
            if (!result.Succeeded)
            {
                return new RegistrationResult { IsSuccess = false, ErrorMessage = "Registration failed. Please try again." };
            }
            // Ajouter un rôle à l'utilisateur
            var roleResult = await _userManager.AddToRoleAsync(appUser, "usernormal");
            if (!roleResult.Succeeded)
            {
                return new RegistrationResult { IsSuccess = false, ErrorMessage = "Failed to assign role to user." };
            }
            // Générer un token de confirmation d'email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            var confirmationLink = $"{_configuration["JWT:Audience"]}/activateaccount?userId={appUser.Id}&token={Uri.EscapeDataString(token)}";
            var emailBody = $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>";
            // Envoyer l'email de confirmation
            await _emailSender.SendEmailAsync(appUser.Email, "Email Confirmation", emailBody);
            return new RegistrationResult { IsSuccess = true };
        }

        public async Task<IActionResult> LogIn(LoginDto login) 
        {
            var user = await _userManager.FindByEmailAsync(login.email);
            if (user == null)
            {
                return new UnauthorizedObjectResult("Invalid email or password.");
            }

            if (!await _userManager.CheckPasswordAsync(user, login.password))
            {
                return new UnauthorizedObjectResult("Invalid email or password.");
            }

            if (!user.EmailConfirmed)
            {
                return new BadRequestObjectResult(new { error = "Email is not yet confirmed." });
            }

            var token = await _tokenService.GenerateJwtToken(user);
            return new OkObjectResult(new { Token = token });
        }

      
        public async Task<IActionResult> ConfirmEmail(Activate dto)
        {
            if (string.IsNullOrWhiteSpace(dto.userId) || string.IsNullOrWhiteSpace(dto.token))
            {
                return new BadRequestObjectResult(new { message = "User ID or token is missing." });
            }

            var user = await _userManager.FindByIdAsync(dto.userId);
            if (user == null)
            {
                return new BadRequestObjectResult(new { message = "User not found." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, dto.token);
            if (result.Succeeded)
            {
                return new OkObjectResult(new { message = "Email confirmed successfully." });
            }

            return new BadRequestObjectResult(new { message = "Email confirmation failed." });
        }

        public async Task<IActionResult> RequestPasswordReset(ResetPasswordRequest req)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(req.Email);
                if (user == null)
                {
                    return new BadRequestObjectResult(new { message = "User not found." });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{_configuration["JWT:Audience"]}/resetpassword?token={Uri.EscapeDataString(token)}&email={req.Email}";
                var emailBody = $"Please reset your password by clicking this link: <a href='{resetLink}'>Reset Password</a>";

                await _emailSender.SendEmailAsync(req.Email, "Password Reset", emailBody);

                return new OkObjectResult(new { message = "Password reset link sent to your email." });
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> ResetPassword(ResetPassword request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return new BadRequestObjectResult(new { message = "Invalid request data." });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new BadRequestObjectResult(new { message = "User not found." });
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (result.Succeeded)
            {
                return new OkObjectResult(new { message = "Password has been reset successfully." });
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return new BadRequestObjectResult(new { Errors = errors });
        }
    }
}