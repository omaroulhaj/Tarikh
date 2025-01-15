using TarikhMaghribi.Models.Entities;

namespace TarikhMaghribi.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(AppUser user);

    }
}
