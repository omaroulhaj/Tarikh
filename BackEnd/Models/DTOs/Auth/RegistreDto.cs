using System.ComponentModel.DataAnnotations;

namespace TarikhMaghribi.Models.DTOs.Auth
{
    public class RegistreDto
    {
        [Required]
        public string prenom { get; set; }
        [Required]
        public string nom { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string email { get; set; }

        [Required]
        public string phoneNumber { get; set; }
        [Required]
        public DateTime dateDeNaissance { get; set; }

    }
}
