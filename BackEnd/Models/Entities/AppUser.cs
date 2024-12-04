using Microsoft.AspNetCore.Identity;

namespace TarikhMaghribi.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public DateTime DateDeNaissance { get; set; }

    }
}
