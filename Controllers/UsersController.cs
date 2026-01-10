using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.DTOs;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")] // Dostęp tylko dla roli Administrator
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        // Wstrzykujemy UserManager
        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // 1. GET: Pobierz wszystkich użytkowników
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();
            
            // Mapowanie na DTO (w produkcji użyj np. AutoMapper)
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles
                });
            }

            return Ok(userDtos);
        }

        // 2. GET: Pobierz po ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("Użytkownik nie istnieje.");

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles
            });
        }

        // 3. POST: Utwórz nowego użytkownika
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = true // Jako admin zakładamy, że mail jest poprawny
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                // Zwracamy błędy (np. za słabe hasło, zajęty email)
                return BadRequest(result.Errors);
            }

            // Opcjonalnie: Przypisz rolę
            if (!string.IsNullOrEmpty(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        // 4. PUT: Edytuj użytkownika
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return NoContent();
        }

        // 5. DELETE: Usuń użytkownika
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Opcjonalne zabezpieczenie: nie pozwól usunąć samego siebie
            // var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // if (user.Id == currentUserId) return BadRequest("Nie możesz usunąć swojego konta.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return NoContent();
        }
    }
}