using TarikhMaghribi.Models.DTOs;

namespace TarikhMaghribi.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserDto>> GetUsers(string currentUserId);
        Task<UserDetailsDto> GetUserDetails(string userId);
        Task<bool> DeleteUser(string userId);
    }
}
