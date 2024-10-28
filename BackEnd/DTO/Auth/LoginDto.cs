using System.ComponentModel.DataAnnotations;

namespace TarikhMaghribi.DTO.Auth
{
    public class LoginDto
    {
        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}
