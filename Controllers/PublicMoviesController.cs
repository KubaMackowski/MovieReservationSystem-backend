using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Data;
using MovieReservationSystem.DTOs;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/public/movies")] 
    public class PublicMoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PublicMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies([FromQuery] int? genreId)
        {
            var query = _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .AsQueryable();

            if (genreId.HasValue)
            {
               
                query = query.Where(m => m.MovieGenres.Any(mg => mg.Genre_Id == genreId.Value));
            }
            else
            {
                
                query = query.OrderByDescending(m => m.Relase_Date);
            }

            var movies = await query
                .Take(20) 
                .ToListAsync();

            var movieDtos = movies.Select(m => new MovieDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                Status = m.Status,
                Relase_Date = m.Relase_Date,
                Duration = m.Duration,
                Director = m.Director,
                Production = m.Production,
                Cast = m.Cast,
                Poster = m.PosterPath ?? "",
                Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
            }).ToList();

            return Ok(movieDtos);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDto>> GetMovieDetails(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound("Film nie został znaleziony.");

            return Ok(new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Status = movie.Status,
                Relase_Date = movie.Relase_Date,
                Duration = movie.Duration,
                Director = movie.Director,
                Production = movie.Production,
                Cast = movie.Cast,
                Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList()
            });
        }
    }
}