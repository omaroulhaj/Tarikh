namespace TarikhMaghribi.Models.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; }
        public string AccountStatus { get; set; }
    }
    public class UserDetailsDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public List<string> Roles { get; set; }
    }

}
