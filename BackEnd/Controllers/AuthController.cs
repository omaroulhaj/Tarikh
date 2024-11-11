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
        private readonly IConfiguration configuration;
        private readonly ISenderEmail emailSender;

        public AuthController(UserManager<AppUser> userManager, AppDbContext context, IConfiguration configuration, RoleManager<IdentityRole> roleManager,ISenderEmail emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            this.configuration = configuration;
            this.emailSender = emailSender;


        }

        [HttpPost("registre")]
        public async Task<IActionResult> RegisterNewUser(RegistreDto user)
        {
            var emailExists = await _userManager.FindByEmailAsync(user.email) != null;

            if (emailExists)
            {
                return BadRequest(new { errors = new { email = " L'email est déjà utilisé." } });
            }

            if (!ModelState.IsValid)
            {

                return BadRequest();
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

            IdentityResult result = await _userManager.CreateAsync(appUser, user.password);
            if (result.Succeeded)
            {
                IdentityResult roleResult = await _userManager.AddToRoleAsync(appUser, "usernormal");

                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("role", "Erreur lors de l'ajout du rôle à l'utilisateur.");
                    return BadRequest(new { errors = new { role = "Erreur lors de l'ajout du rôle à l'utilisateur." } });
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                var confirmationLink = $"{configuration["JWT:Audience"]}/activateaccount?userId={appUser.Id}&token={Uri.EscapeDataString(token)}";
                var emailBody = $"Veuillez confirmer votre e-mail en cliquant sur ce lien : <a href='{confirmationLink}'>Confirmer l'e-mail</a>";
                await emailSender.SendEmailAsync(appUser.Email, "Confirmation de votre e-mail", emailBody);
                return Ok(new { message = "Success", token });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LoginDto login)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AppUser? user = await _userManager.FindByEmailAsync(login.email);
                    if (user != null)
                    {
                        if (await _userManager.CheckPasswordAsync(user, login.password))
                        {
                            if (!user.EmailConfirmed)
                            {
                                return BadRequest(new { error="Veuillez vérifier votre e-mail avant de vous connecter." });
                            }
                            var token = await GenerateJwtToken(user);
                            return Ok(new { Token = token });
                        }
                        else
                        {
                            return Unauthorized("Email ou mot de passe incorrect");
                        }
                    }
                    else
                    {
                        return Unauthorized("An error occurred while processing your request.");
                    }
                }
                catch
                {
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }
            return BadRequest();
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("requestpasswordreset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{configuration["JWT:Audience"]}/resetpassword?token={Uri.EscapeDataString(token)}&email={email}";

            var emailBody = $"Please reset your password by clicking this link: <a href='{resetLink}'>Reset Password</a>";
            await emailSender.SendEmailAsync(email, "Password Reset", emailBody);

            return Ok("Password reset link sent.");
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Token) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("Invalid request data.");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password has been reset successfully.");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { Errors = errors });
        }
    }

    
}

