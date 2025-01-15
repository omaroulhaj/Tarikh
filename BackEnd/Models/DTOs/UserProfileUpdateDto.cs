namespace TarikhMaghribi.Models.DTOs
{
    public class UserProfileUpdateDto
    {
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateDeNaissance { get; set; }
    }
}
