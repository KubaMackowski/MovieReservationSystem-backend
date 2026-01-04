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
        // Zmiana 1: Zamiast SecurityKey używamy IConfiguration
        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Pamiętaj: lista statyczna resetuje się po restarcie aplikacji!
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

            // Zmiana 2: Tworzymy klucz tutaj, pobierając go z konfiguracji
            var keyString = _configuration["Jwt:Key"];
            
            // Zabezpieczenie na wypadek braku klucza w appsettings
            if(string.IsNullOrEmpty(keyString)) return StatusCode(500, "Błąd serwera: brak klucza JWT w konfiguracji");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                // Używamy stworzonego wyżej klucza
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"], // Opcjonalnie: dodaj Issuer
                Audience = _configuration["Jwt:Audience"] // Opcjonalnie: dodaj Audience
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