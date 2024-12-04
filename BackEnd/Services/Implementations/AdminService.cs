using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TarikhMaghribi.DBContext;
using TarikhMaghribi.Models.DTOs;
using TarikhMaghribi.Models.Entities;
using TarikhMaghribi.Services.Interfaces;

namespace TarikhMaghribi.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminService(AppDbContext context,UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<List<UserDto>> GetUsers(string currentUserId)
        {
            var users = await _userManager.Users
                .Where(user => user.Id != currentUserId)
                .ToListAsync();

            var result = new List<UserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    Username = $"{user.Prenom} {user.Nom}",
                    Email = user.Email,
                    Prenom = user.Prenom,
                    Nom = user.Nom,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles.ToList(),
                    AccountStatus = user.EmailConfirmed ? "Activated" : "Pending"
                });
            }

            return result;
        }
        public async Task<UserDetailsDto> GetUserDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null; // Ou lancez une exception personnalisée si vous préférez
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDetailsDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Prenom = user.Prenom,
                Nom = user.Nom,
                Roles = roles.ToList()
            };
        }
        public async Task<bool> DeleteUser(string userId)
        {
            // Rechercher l'utilisateur
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Utilisateur non trouvé.");
            }

            // Supprimer les tâches associées à l'utilisateur
            var userTasks = _context.Tasks.Where(t => t.UserId == userId);
            _context.Tasks.RemoveRange(userTasks);

            // Sauvegarder les modifications dans la base de données
            await _context.SaveChangesAsync();

            // Supprimer l'utilisateur
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Échec de la suppression de l'utilisateur.");
            }

            return true;
        }

    }
}
