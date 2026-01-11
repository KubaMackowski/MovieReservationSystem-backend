using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Data;
using MovieReservationSystem.DTOs;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/public/movies")] // Inna ścieżka niż dla admina
    public class PublicMoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PublicMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/public/movies
        // Obsługuje dwa scenariusze:
        // 1. ?genreId=5 -> Zwraca filmy z kategorii ID 5
        // 2. (brak parametru) -> Zwraca 20 najnowszych filmów
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies([FromQuery] int? genreId)
        {
            var query = _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .AsQueryable();

            if (genreId.HasValue)
            {
                // Scenariusz A: Użytkownik kliknął w kategorię
                query = query.Where(m => m.MovieGenres.Any(mg => mg.Genre_Id == genreId.Value));
            }
            else
            {
                // Scenariusz B: Strona główna (Nowości)
                query = query.OrderByDescending(m => m.Relase_Date);
            }

            var movies = await query
                .Take(20) // Limit do 20 wyników
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
                Poster = m.Poster?.File_Name,
                Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
            }).ToList();

            return Ok(movieDtos);
        }

        // GET: api/public/movies/5
        // Szczegóły filmu (dla zwykłego użytkownika)
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