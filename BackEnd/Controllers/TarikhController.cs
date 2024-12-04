using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TarikhMaghribi.DBContext;
using Microsoft.EntityFrameworkCore;
using TarikhMaghribi.Services.Interfaces;

namespace TarikhMaghribi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarikhController : ControllerBase
    {
        private readonly IJourFerieService _jourFerieService;

        public TarikhController(IJourFerieService jourFerieService)
        {
            _jourFerieService = jourFerieService;
        }

        [HttpGet("jours-feries/{annee}/{mois}")]
        public async Task<IActionResult> GetJoursFeriesByMonthAndYear(int annee, int mois)
        {
            try
            {
                var joursFeries = await _jourFerieService.GetJoursFeriesByMonthAndYear(annee, mois);

                return Ok(joursFeries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne du serveur : {ex.Message}");
            }
        }
    }
}
