using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly SecurityKey _signingKey;

        public UsersController(SecurityKey signingKey)
        {
            _signingKey = signingKey;
        }
        private static List<User> users = new List<User>();

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            users.Add(user);
            return Ok(new { message = "Użytkownik dodany" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            var user = users.FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);
            if (user == null) return Unauthorized(new { message = "Nieprawidłowe dane" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt });
        }

        [HttpGet("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Profile()
        {
            var username = User.Identity!.Name;
            return Ok(new { message = "Twój profil", username });
        }
    }

    public class User
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}