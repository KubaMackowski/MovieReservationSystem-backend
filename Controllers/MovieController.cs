using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Data; // <--- Twój namespace DbContext
using MovieReservationSystem.DTOs;
using MovieReservationSystem.Models;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")] // Tylko Admin zarządza filmami
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/movies
        [HttpGet]
        [AllowAnonymous] // Każdy może zobaczyć listę filmów
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetAll()
        {
            var movies = await _context.Movies
                .Include(m => m.MovieGenres) // Dołącz tabelę łączącą
                    .ThenInclude(mg => mg.Genre) // Dołącz tabelę gatunków
                .ToListAsync();

            // Mapowanie na DTO
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
                // Wyciągamy same nazwy gatunków
                Genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList()
            }).ToList();

            return Ok(movieDtos);
        }

        // 2. GET: api/movies/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<MovieDto>> GetById(int id)
{
    var movie = await _context.Movies
        .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
        .Include(m => m.Showings).ThenInclude(s => s.Room).ThenInclude(r => r.SeatsList)
        .Include(m => m.Showings).ThenInclude(s => s.Reservations) // Potrzebne do sprawdzenia zajętości
        .Include(m => m.Showings).ThenInclude(s => s.Prices)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (movie == null) return NotFound("Film nie został znaleziony.");

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
        PosterPath =  movie.PosterPath ?? "",
        Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
        
        // --- MAPOWANIE RĘCZNE (PRZERYWA PĘTLĘ) ---
        Showings = movie.Showings?.Select(s => new MShowingDto
        {
            Id = s.Id,
            Date = s.Date,
            End_Date = s.End_Date,
            Price = s.Prices?.FirstOrDefault()?.PriceValue ?? 0, // Pobieramy cenę
            
            Room = new MRoomDto
            {
                Id = s.Room.Id,
                Number = s.Room.Number,
                // Mapujemy miejsca i sprawdzamy czy są zajęte w TYM seansie
                Seats = s.Room.SeatsList.Select(seat => new MSeatDto
                {
                    Id = seat.Id,
                    Row = seat.Row,
                    Number = seat.Number,
                    // Sprawdzamy czy ID miejsca znajduje się na liście rezerwacji tego seansu
                    IsOccupied = s.Reservations.Any(r => r.Seat_Id == seat.Id)
                }).OrderBy(seat => seat.Row).ThenBy(seat => seat.Number).ToList()
            }
        }).OrderBy(s => s.Date).ToList() ?? new List<MShowingDto>()
    };

    return Ok(movieDto);
}

        // 3. POST: api/movies
        [HttpPost]
        public async Task<ActionResult<MovieDto>> Create([FromBody] CreateMovieDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Tworzymy nowy obiekt filmu
            var movie = new Movie
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                Created_At = DateTime.UtcNow, // Ustawiamy datę utworzenia automatycznie
                Relase_Date = model.Relase_Date,
                Duration = model.Duration,
                Director = model.Director,
                Production = model.Production,
                Cast = model.Cast,
                PosterPath = model.PosterPath,
                
                // Inicjalizujemy puste kolekcje, bo są "required" w Twoim modelu
                MovieGenres = new List<MovieGenre>(),
                Showings = new List<Showing>()
            };

            // Obsługa przypisywania gatunków (relacja wiele-do-wielu)
            if (model.GenreIds != null && model.GenreIds.Any())
            {
                // Pobieramy gatunki z bazy, żeby upewnić się, że istnieją
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

        // 4. PUT: api/movies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMovieDto model)
        {
            var movie = await _context.Movies
                .Include(m => m.MovieGenres) // Ważne przy edycji relacji
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound("Film nie istnieje.");

            // Aktualizacja pól prostych
            movie.Title = model.Title;
            movie.Description = model.Description;
            movie.Status = model.Status;
            movie.Relase_Date = model.Relase_Date;
            movie.Duration = model.Duration;
            movie.Director = model.Director;
            movie.Production = model.Production;
            movie.Cast = model.Cast;
            movie.PosterPath = model.PosterPath;

            // Aktualizacja gatunków (jeśli podano nową listę)
            if (model.GenreIds != null)
            {
                // 1. Usuń stare powiązania
                movie.MovieGenres.Clear();

                // 2. Dodaj nowe powiązania
                var genres = await _context.Genres
                    .Where(g => model.GenreIds.Contains(g.Id))
                    .ToListAsync();

                foreach (var genre in genres)
                {
                    movie.MovieGenres.Add(new MovieGenre 
                    { 
                        Genre_Id = genre.Id,
                        Movie_Id = movie.Id, // Warto też przypisać ID filmu
                        Movie = movie,       // <--- Tego brakowało (wymagane przez 'required')
                        Genre = genre        // <--- Tego brakowało (wymagane przez 'required')
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 5. DELETE: api/movies/5
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