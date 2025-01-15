using System.ComponentModel.DataAnnotations;

namespace TarikhMaghribi.Models.Entities
{
    public class JourFerie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; }

        public string Details { get; set; }

        [Required]
        public DateTime DateJour { get; set; }

        [Required]
        [MaxLength(50)]
        public string Categorie { get; set; } // Par exemple "watani" ou "dini"
    }
}
