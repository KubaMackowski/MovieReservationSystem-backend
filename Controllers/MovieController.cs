using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Data; 
using MovieReservationSystem.DTOs;
using MovieReservationSystem.Models;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")] 
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        [AllowAnonymous] 
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetAll()
        {
            var movies = await _context.Movies
                .Include(m => m.MovieGenres) 
                    .ThenInclude(mg => mg.Genre) 
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
                PosterPath =  m.PosterPath ?? "",
                
                Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
            }).ToList();

            return Ok(movieDtos);
        }
      
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<MovieDto>> GetById(int id)
{
    var movie = await _context.Movies
        .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
        .Include(m => m.Showings).ThenInclude(s => s.Room).ThenInclude(r => r.SeatsList)
        .Include(m => m.Showings).ThenInclude(s => s.Reservations)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (movie == null) return NotFound("Film nie został znaleziony.");

    
    var currentTime = DateTime.UtcNow; 

    var movieDto = new MovieDto
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
        PosterPath = movie.PosterPath ?? "",
        Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),

       
        Showings = movie.Showings?
            .Where(s => s.Date > currentTime) 
            .Select(s => new MShowingDto
            {
                Id = s.Id,
                Date = s.Date,
                End_Date = s.End_Date,
                Price = s.Price,
                Room = new MRoomDto
                {
                    Id = s.Room.Id,
                    Number = s.Room.Number,
                    Seats = s.Room.SeatsList.Select(seat => new MSeatDto
                    {
                        Id = seat.Id,
                        Row = seat.Row,
                        Number = seat.Number,
                        IsOccupied = s.Reservations.Any(r => r.Seat_Id == seat.Id)
                    }).OrderBy(seat => seat.Row).ThenBy(seat => seat.Number).ToList()
                }
            })
            .OrderBy(s => s.Date)
            .ToList() ?? new List<MShowingDto>()
    };

    return Ok(movieDto);
}

        
        [HttpPost]
        public async Task<ActionResult<MovieDto>> Create([FromBody] CreateMovieDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            
            var movie = new Movie
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                Created_At = DateTime.UtcNow, 
                Relase_Date = model.Relase_Date,
                Duration = model.Duration,
                Director = model.Director,
                Production = model.Production,
                Cast = model.Cast,
                PosterPath = model.PosterPath,
                
                
                MovieGenres = new List<MovieGenre>(),
                Showings = new List<Showing>()
            };

            
            if (model.GenreIds != null && model.GenreIds.Any())
            {
                
                var genres = await _context.Genres
                    .Where(g => model.GenreIds.Contains(g.Id))
                    .ToListAsync();

                foreach (var genre in genres)
                {
                    movie.MovieGenres.Add(new MovieGenre 
                    { 
                        Genre = genre, 
                        Movie = movie 
                    });
                }
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, model);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMovieDto model)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound("Film nie istnieje.");

            
            movie.Title = model.Title;
            movie.Description = model.Description;
            movie.Status = model.Status;
            movie.Relase_Date = model.Relase_Date;
            movie.Duration = model.Duration;
            movie.Director = model.Director;
            movie.Production = model.Production;
            movie.Cast = model.Cast;
            movie.PosterPath = model.PosterPath;

           
            if (model.GenreIds != null)
            {
                
                movie.MovieGenres.Clear();

                
                var genres = await _context.Genres
                    .Where(g => model.GenreIds.Contains(g.Id))
                    .ToListAsync();

                foreach (var genre in genres)
                {
                    movie.MovieGenres.Add(new MovieGenre 
                    { 
                        Genre_Id = genre.Id,
                        Movie_Id = movie.Id, 
                        Movie = movie,       
                        Genre = genre       
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound("Film nie istnieje.");

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}