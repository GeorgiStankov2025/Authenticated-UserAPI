using AuthUserAPI.Data;
using AuthUserAPI.Entities;
using AuthUserAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthUserAPI.Services
{
    public class AuthService(UserDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<TokenResponseDTO> LoginAsync(UserDTO request)
        {

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user is null)
            {

                return null;

            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {

                return null;

            }

            

            return await CreatetokenResponse(user);
        }

        private async Task<TokenResponseDTO> CreatetokenResponse(User user)
        {
            return new TokenResponseDTO { AccessToken = CreateToken(user), RefreshToken = await GenerateAndSaveRefreshtokenAsync(user) };
        }

        public async Task<User?> RegisterAsync(UserDTO request)
        {
            if(await context.Users.AnyAsync(u=>u.Username==request.Username))
            {

                return null;

            }

            var user= new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            context.Add(user);

            await context.SaveChangesAsync();

            return user;
        }

        private string CreateToken(User user)
        {

            var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(

                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds

            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }

        private string GenerateRefreshtoken()
        {

            var randomNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);

        }

        private async Task<string> GenerateAndSaveRefreshtokenAsync(User user)
        {

            var refreshToken= GenerateRefreshtoken();

            user.RefreshToken= refreshToken;

            user.RefreshTokenExpiryTime= DateTime.UtcNow.AddDays(7);

            await context.SaveChangesAsync();

            return refreshToken;

        }

        private async Task<User?> ValidateRefreshtokenAsync(Guid userId, string refreshToken)
        {

            var user = await context.Users.FindAsync(userId);

            if(user is null || user.RefreshToken != refreshToken||user.RefreshTokenExpiryTime<=DateTime.UtcNow)
            {

                return null;

            }

            return user;

        }

        public async Task<TokenResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {
            var user = await ValidateRefreshtokenAsync(request.UserId, request.RefreshToken);
                
            if(user is null)
            {
                return null;

            }

            return await CreatetokenResponse(user);
            
            

        }
    }
}
