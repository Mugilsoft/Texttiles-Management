using System.Threading.Tasks;
using TextileBilling.Core.DTOs.Auth;

namespace TextileBilling.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<string> GenerateJwtToken(Entities.ApplicationUser user);
    }
}
