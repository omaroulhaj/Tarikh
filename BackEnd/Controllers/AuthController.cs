using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TarikhMaghribi.DBContext;
using TarikhMaghribi.Models.DTOs.Auth;
using TarikhMaghribi.Services.Interfaces;

namespace TarikhMaghribi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewUser(RegistreDto user)
        {
            // Vérification de la validité des données envoyées dans le corps de la requête
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data provided." });
            }
            // Appel du service pour enregistrer le nouvel utilisateur
            var result = await _authService.RegisterNewUser(user);

            if (result.IsSuccess)
            {
                return Ok(new { message = "Registration successful. Please check your email for confirmation." });
            }
            else
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LoginDto login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login data.");
            }

           return await _authService.LogIn(login);
        }

       
        [HttpPost("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(Activate dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid activate data.");
            }
            return await _authService.ConfirmEmail(dto);
        }


        [HttpPost("requestpasswordreset")]
        public async Task<IActionResult> RequestPasswordReset(ResetPasswordRequest req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }
            return await _authService.RequestPasswordReset(req);
        }


        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }
            return await _authService.ResetPassword(request);
        }
    }
}
