using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Data;
using MovieReservationSystem.DTOs;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/public/genres")] 
    public class PublicGenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PublicGenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetLatestGenres()
        {
            var genres = await _context.Genres
                .OrderByDescending(g => g.Id) 
                .Take(7)                        
                .Select(g => new GenreDto 
                { 
                    Id = g.Id, 
                    Name = g.Name 
                })
                .ToListAsync();

            return Ok(genres);
        }
        
        
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