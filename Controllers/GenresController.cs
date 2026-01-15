using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.DTOs;
using MovieReservationSystem.Models;
using MovieReservationSystem.Data; 

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    [Authorize(Roles = "ADMIN")] 
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        [AllowAnonymous] 
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

        
        [HttpGet("{id}")]
        [AllowAnonymous] 
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

        
        [HttpPost]
        public async Task<ActionResult<GenreDto>> Create([FromBody] CreateGenreDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            
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

            
            var genreDto = new GenreDto { Id = genre.Id, Name = genre.Name };

            return CreatedAtAction(nameof(GetById), new { id = genre.Id }, genreDto);
        }

        
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

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound("Gatunek nie istnieje.");

            

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