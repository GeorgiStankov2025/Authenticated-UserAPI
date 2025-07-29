using AuthUserAPI.Entities;
using AuthUserAPI.Models;
using AuthUserAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthUserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        

        [HttpPost("register")]

        public async Task<ActionResult<User>> Register(UserDTO request)
        {
            var user = await authService.RegisterAsync(request);

            if(user is null)
            {

                return BadRequest("Username already exists.");

            }
            
            return Ok(user);

        }

        [HttpPost("login")]

        public async Task<ActionResult<TokenResponseDTO>> Login (UserDTO request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {

                return BadRequest("Invalid username or password.");

            }
            

            return Ok(result);

        }
        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndPoint()
        {

            return Ok("you are authenticated");

        }

        [Authorize(Roles ="Admin")]
        [HttpGet("admin-only")]
        public IActionResult AAdminOnlyEndPoint()
        {

            return Ok("you are an admin");

        }

        [HttpPost("refresh-token")]

        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request)
        {

            var result= await authService.RefreshTokenAsync(request);
            if(result is null || result.AccessToken is null || result.RefreshToken is null)
            {

                return Unauthorized("Invalid refresh token");

            }

            return Ok(result);

        }
    }
}
