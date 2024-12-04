namespace TarikhMaghribi.Models.DTOs
{
    public class UserProfileDto
    {
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateDeNaissance { get; set; }
        public IList<string> Roles { get; set; }
    }
}
