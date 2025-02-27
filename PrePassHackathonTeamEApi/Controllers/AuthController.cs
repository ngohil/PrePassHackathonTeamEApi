using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PrePassHackathonTeamEApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private const string SecretKey = "this-is-a-dummy-secret-key-change-it"; // Dummy secret key

        private readonly AppSettings _appSettings;

        public AuthController(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        [HttpPost("login")]
        public IActionResult Login()
        {
            // Create dummy claims
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, "dummyUser"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwToken));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60), // Valid for 60 minutes
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
