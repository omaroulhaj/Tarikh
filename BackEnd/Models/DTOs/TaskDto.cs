using System.ComponentModel.DataAnnotations;

namespace TarikhMaghribi.Models.DTOs
{
    public class TaskDto
    {
        [Required]
        public DateTime Date { get; set; }  // Date complète avec heure
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }  // "en cours", "terminé", "pas en cours"
    }
}
