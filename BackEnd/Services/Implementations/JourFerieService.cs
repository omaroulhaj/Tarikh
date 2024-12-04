using TarikhMaghribi.DBContext;
using Microsoft.EntityFrameworkCore;
using TarikhMaghribi.Services.Interfaces;
using TarikhMaghribi.Models.DTOs;
using TarikhMaghribi.Models.Entities;

namespace TarikhMaghribi.Services.Implementations
{
    public class JourFerieService : IJourFerieService
    {
        private readonly AppDbContext _context;
        public JourFerieService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateJourFerie(JourFerieDto jourFerieDto)
        {
            var jourFerie = new JourFerie
            {
                Nom = jourFerieDto.Nom,
                Details = jourFerieDto.Details,
                DateJour = jourFerieDto.DateJour,
                Categorie = jourFerieDto.Categorie
            };
            _context.JoursFeries.Add(jourFerie);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateJourFerie(int id, JourFerieDto jourFerieDto)
        {
            var jourFerie = await _context.JoursFeries.FindAsync(id);
            if (jourFerie == null)
            {
                return false; // Indique que l'entité n'a pas été trouvée
            }

            // Met à jour les propriétés
            jourFerie.Nom = jourFerieDto.Nom;
            jourFerie.Details = jourFerieDto.Details;
            jourFerie.DateJour = jourFerieDto.DateJour;
            jourFerie.Categorie = jourFerieDto.Categorie;

            try
            {
                _context.JoursFeries.Update(jourFerie);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false; // Indique qu'il y a eu un problème lors de la sauvegarde
            }
        }
        public async Task<bool> DeleteJourFerie(int id)
        {
            var jourFerie = await _context.JoursFeries.FindAsync(id);
            if (jourFerie == null)
            {
                return false; // Indique que l'entité n'existe pas
            }

            try
            {
                _context.JoursFeries.Remove(jourFerie);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false; // Indique qu'une erreur est survenue
            }
        }
        public async Task<List<JourFerie>> GetAllJoursFeries()
        {
            return await _context.JoursFeries
                .OrderByDescending(jf => jf.DateJour)
                .ToListAsync();
        }
        public async Task<List<JourFerie>> GetJoursFeriesByMonthAndYear(int annee, int mois)
        {
            return await _context.JoursFeries
                .Where(jf => jf.DateJour.Year == annee && jf.DateJour.Month == mois)
                .OrderBy(jf => jf.DateJour)
                .ToListAsync();
        }

    }
}
