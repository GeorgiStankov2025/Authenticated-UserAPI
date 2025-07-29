using AuthUserAPI.Entities;
using AuthUserAPI.Models;
namespace AuthUserAPI.Services
{
    public interface IAuthService
    {

        Task<User?> RegisterAsync(UserDTO request);

        Task<TokenResponseDTO> LoginAsync(UserDTO request);

        Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request);

    }
}
