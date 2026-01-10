using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Data;
using MovieReservationSystem.DTOs;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/public/genres")] // Inna ścieżka niż dla admina
    public class PublicGenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PublicGenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/public/genres/latest
        // Pobiera 7 ostatnio dodanych kategorii (do paska nawigacji)
        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetLatestGenres()
        {
            var genres = await _context.Genres
                .OrderByDescending(g => g.Id) // Najnowsze = najwyższe ID
                .Take(7)                        // Tylko 7 sztuk
                .Select(g => new GenreDto 
                { 
                    Id = g.Id, 
                    Name = g.Name 
                })
                .ToListAsync();

            return Ok(genres);
        }
        
        // GET: api/public/genres
        // Opcjonalnie: Lista wszystkich kategorii do filtrów
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll()
        {
            var genres = await _context.Genres
                .Select(g => new GenreDto { Id = g.Id, Name = g.Name })
                .ToListAsync();
            return Ok(genres);
        }
    }
}