using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TarikhMaghribi.Models.Entities
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }  // "en cours", "terminé", "pas en cours"
        [Required]
        public string UserId { get; set; }  // Clé étrangère pour lier la tâche à un utilisateur

        [ForeignKey("UserId")]
        public AppUser User { get; set; }  // Propriété de navigation vers l'utilisateur
    }
}
