using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MovieReservationSystem.DTOs;
// Nie potrzebujemy tu już ApplicationDbContext ani BCrypt, 
// bo wszystko załatwi UserManager (to kluczowe dla działania logowania!)

namespace MovieReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        // --- TO JEST JEDYNY KONSTRUKTOR ---
        // Przyjmuje tylko to, co niezbędne do Auth.
        // Jeśli potrzebujesz Contextu do innych rzeczy, dopisz go tutaj, 
        // ale NIE twórz drugiego konstruktora "public AuthController(...)".
        public AuthController(
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        // ----------------------------------

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Tworzymy użytkownika IdentityUser
            var user = new IdentityUser
            {
                UserName = request.Email, // Identity wymaga UserName
                Email = request.Email
            };

            // Ta linijka:
            // 1. Sprawdza czy email jest wolny
            // 2. Haszuje hasło bezpiecznie
            // 3. Zapisuje usera do tabeli AspNetUsers
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "Rejestracja przebiegła pomyślnie." });
            }

            // Obsługa błędów (np. hasło bez cyfry, email zajęty)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Szukamy usera w systemie Identity
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null)
            {
                return Unauthorized("Błędny login lub hasło.");
            }

            // Sprawdzamy hasło
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(new { message = "Zalogowano pomyślnie" });
            }

            if (result.IsLockedOut)
            {
                return StatusCode(423, "Konto zablokowane.");
            }

            return Unauthorized("Błędny login lub hasło.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Wylogowano pomyślnie" });
        }
    }
}