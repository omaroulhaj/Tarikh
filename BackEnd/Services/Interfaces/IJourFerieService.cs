using TarikhMaghribi.Models.DTOs;
using TarikhMaghribi.Models.Entities;

namespace TarikhMaghribi.Services.Interfaces
{
    public interface IJourFerieService
    {
        Task<List<JourFerie>> GetAllJoursFeries(); // Pour admin
        Task<List<JourFerie>> GetJoursFeriesByMonthAndYear(int annee, int mois); // Pour tous les user 
        Task<bool> CreateJourFerie(JourFerieDto jourFerieDto);
        Task<bool> UpdateJourFerie(int id, JourFerieDto jourFerieDto);
        Task<bool> DeleteJourFerie(int id);
    }
}
