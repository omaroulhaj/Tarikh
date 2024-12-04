using System.ComponentModel.DataAnnotations;

namespace TarikhMaghribi.Models.DTOs
{
    public class JourFerieDto
    {
        [Required]
        public string Nom { get; set; }

        public string Details { get; set; }

        [Required]
        public DateTime DateJour { get; set; }

        [Required]
        public string Categorie { get; set; } // "watani" ou "dini"
    }
}
