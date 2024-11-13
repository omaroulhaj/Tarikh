using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TarikhMaghribi.DBContext;
using TarikhMaghribi.DBContext.Models;
using TarikhMaghribi.DTO.Auth;
using static TarikhMaghribi.Extentions.Mail;

namespace TarikhMaghribi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ISenderEmail _emailSender;

        public AuthController(UserManager<AppUser> userManager, AppDbContext context, IConfiguration configuration, RoleManager<IdentityRole> roleManager, ISenderEmail emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser(RegistreDto user)
        {
            if (await _userManager.FindByEmailAsync(user.email) != null)
            {
                return BadRequest(new { errors = new { email = "Email is already in use." } });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data provided.");
            }

            AppUser appUser = new()
            {
                UserName = user.email,
                Email = user.email,
                Prenom = user.prenom,
                Nom = user.nom,
                PhoneNumber = user.phoneNumber,
                DateDeNaissance = user.dateDeNaissance,
            };

            var result = await _userManager.CreateAsync(appUser, user.password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(appUser, "usernormal");
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { errors = new { role = "Failed to assign role to user." } });
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                var confirmationLink = $"{_configuration["JWT:Audience"]}/activateaccount?userId={appUser.Id}&token={Uri.EscapeDataString(token)}";
                var emailBody = $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>";
                await _emailSender.SendEmailAsync(appUser.Email, "Email Confirmation", emailBody);

                return Ok(new { message = "Registration successful. Please check your email for confirmation." });
            }

            return BadRequest("Registration failed. Please try again.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LoginDto login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login data.");
            }

            var user = await _userManager.FindByEmailAsync(login.email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            if (!await _userManager.CheckPasswordAsync(user, login.password))
            {
                return Unauthorized("Invalid email or password.");
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest(new { error = "Email is not yet confirmed." });
            }

            var token = await GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(Activate dto)
        {
            if (string.IsNullOrWhiteSpace(dto.userId) || string.IsNullOrWhiteSpace(dto.token))
            {
                return BadRequest(new { message = "User ID or token is missing." });
            }

            var user = await _userManager.FindByIdAsync(dto.userId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, dto.token);
            if (result.Succeeded)
            {
                return Ok(new { message = "Email confirmed successfully." });
            }

            // Ajout d'un message plus détaillé
            if (result.Errors.Any())
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = "Email confirmation failed.", details = errorMessages });
            }

            return BadRequest(new { message = "Email confirmation failed." });
        }


        [HttpPost("requestpasswordreset")]
        public async Task<IActionResult> RequestPasswordReset(ResetPasswordRequest req)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(req.Email);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found." });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{_configuration["JWT:Audience"]}/resetpassword?token={Uri.EscapeDataString(token)}&email={req.Email}";
                var emailBody = $"Please reset your password by clicking this link: <a href='{resetLink}'>Reset Password</a>";

                // Assurez-vous que l'envoi d'email ne pose pas de problème
                await _emailSender.SendEmailAsync(req.Email, "Password Reset", emailBody);

                return Ok(new { message = "Password reset link sent to your email." });
            }
            catch (Exception ex)
            {
                // Log l'erreur ici pour le suivi, et renvoie une erreur générique mais explicite
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }


        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new { message ="Password has been reset successfully." });
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Errors = errors });
        }
    }
}
