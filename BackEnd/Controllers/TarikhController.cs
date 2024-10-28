using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static TarikhMaghribi.Extentions.Mail;
using TarikhMaghribi.DBContext.Models;
using TarikhMaghribi.DBContext;
using Microsoft.EntityFrameworkCore;

namespace TarikhMaghribi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarikhController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration configuration;
        public TarikhController(UserManager<AppUser> userManager, AppDbContext context, IConfiguration configuration, RoleManager<IdentityRole> roleManager, ISenderEmail emailSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            this.configuration = configuration;
        }
        [HttpGet("jours-feries/{annee}/{mois}")]
        public async Task<IActionResult> GetJoursFeriesByMonthAndYear(int annee, int mois)
        {
            try
            {
                var joursFeries = await _context.JoursFeries
                    .Where(jf => jf.DateJour.Year == annee && jf.DateJour.Month == mois)
                    .OrderBy(jf => jf.DateJour)
                    .ToListAsync();

                return Ok(joursFeries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }
    }
}
