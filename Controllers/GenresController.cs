using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.DTOs;
using MovieReservationSystem.Models;
using MovieReservationSystem.Data; // <--- Tutaj musi być namespace Twojego DbContextu

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Zabezpieczamy cały kontroler dla Admina. 
    // Jeśli zwykli użytkownicy mają widzieć gatunki, przenieś ten atrybut tylko nad metody POST/PUT/DELETE
    [Authorize(Roles = "ADMIN")] 
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // <--- Zmień ApplicationDbContext na nazwę Twojego kontekstu

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/genres
        [HttpGet]
        [AllowAnonymous] // Opcjonalnie: Pozwól każdemu pobrać listę gatunków (nawet niezalogowanym)
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll()
        {
            var genres = await _context.Genres
                .Select(g => new GenreDto 
                { 
                    Id = g.Id, 
                    Name = g.Name 
                })
                .ToListAsync();

            return Ok(genres);
        }

        // 2. GET: api/genres/5
        [HttpGet("{id}")]
        [AllowAnonymous] // Opcjonalnie
        public async Task<ActionResult<GenreDto>> GetById(int id)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound("Gatunek nie został znaleziony.");
            }

            return Ok(new GenreDto 
            { 
                Id = genre.Id, 
                Name = genre.Name 
            });
        }

        // 3. POST: api/genres
        [HttpPost]
        public async Task<ActionResult<GenreDto>> Create([FromBody] CreateGenreDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Sprawdź czy taki gatunek już istnieje (opcjonalnie)
            if (await _context.Genres.AnyAsync(g => g.Name == model.Name))
            {
                return BadRequest("Taki gatunek już istnieje.");
            }

            var genre = new Genre
            {
                Name = model.Name
            };

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            // Zwracamy utworzony obiekt
            var genreDto = new GenreDto { Id = genre.Id, Name = genre.Name };

            return CreatedAtAction(nameof(GetById), new { id = genre.Id }, genreDto);
        }

        // 4. PUT: api/genres/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGenreDto model)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound("Gatunek nie istnieje.");

            genre.Name = model.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // 5. DELETE: api/genres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound("Gatunek nie istnieje.");

            // Opcjonalnie: Sprawdź czy gatunek jest przypisany do jakichś filmów
            // var hasMovies = await _context.MovieGenres.AnyAsync(mg => mg.GenreId == id);
            // if (hasMovies) return BadRequest("Nie można usunąć gatunku przypisanego do filmów.");

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
    }
}