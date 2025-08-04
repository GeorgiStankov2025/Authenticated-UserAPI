using AuthUserAPI.Entities;
using AuthUserAPI.Models;
namespace AuthUserAPI.Services
{
    public interface IAuthService
    {

        Task<User?> RegisterAsync(UserDTO request);

        Task<TokenResponseDTO> LoginAsync(UserDTO request);

        Task<string> RefreshTokenAsync(RefreshTokenRequestDTO request);

        Task<TokenResponseDTO> RefreshBothTokensAsync(RefreshTokenRequestDTO request);

        Task<User?> ChangePasswordAsync(UserDTO request, string username,string newPassword, string confirmPassword);

    }
}
