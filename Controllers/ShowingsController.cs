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
    //[Authorize(Roles = "ADMIN")]
    public class ShowingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShowingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/showings
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ShowingDto>>> GetAll()
        {
            var showings = await _context.Showings
                .Include(s => s.Movie) // Pobierz dane filmu (tytuł)
                .Include(s => s.Room)  // Pobierz dane sali (nazwa)
                .OrderBy(s => s.Date)  // Sortuj od najwcześniejszych
                .ToListAsync();

           var showingDtos = showings.Select(s => new ShowingDto
                {
                    Id = s.Id,
                    Date = s.Date,
                    End_Date = s.End_Date,
                    Movie_Id = s.Movie_Id,
                    MovieTitle = s.Movie.Title,
                    Room_Id = s.Room_Id,
                    RoomNumber = s.Room.Number // <--- POPRAWKA TUTAJ
                });

            return Ok(showingDtos);
        }

        // 2. GET: api/showings/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ShowingDto>> GetById(int id)
        {
            var showing = await _context.Showings
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showing == null) return NotFound("Seans nie istnieje.");

           return Ok(new ShowingDto
            {
                Id = showing.Id,
                Date = showing.Date,
                End_Date = showing.End_Date,
                Movie_Id = showing.Movie_Id,
                MovieTitle = showing.Movie.Title,
                Room_Id = showing.Room_Id,
                RoomNumber = showing.Room.Number // <--- POPRAWKA TUTAJ
            });
        }
        // 3. POST: api/showings
        [HttpPost]
        public async Task<ActionResult<ShowingDto>> Create([FromBody] CreateShowingDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Pobierz Film (potrzebny do czasu trwania i required)
            var movie = await _context.Movies.FindAsync(model.Movie_Id);
            if (movie == null) return BadRequest("Podany film nie istnieje.");

            // 2. Pobierz Salę (potrzebna do required)
            var room = await _context.Rooms.FindAsync(model.Room_Id);
            if (room == null) return BadRequest("Podana sala nie istnieje.");

            // 3. Oblicz datę zakończenia (Start + Czas trwania filmu)
            var endDate = model.Date.AddMinutes(movie.Duration);

            // (Opcjonalnie: Tutaj można dodać walidację, czy sala jest wolna w tym czasie)

            // 4. Utwórz obiekt (z przypisaniem całych obiektów Movie i Room)
            var showing = new Showing
            {
                Date = model.Date,
                End_Date = endDate,
                Movie_Id = model.Movie_Id,
                Room_Id = model.Room_Id,
                Movie = movie, // <--- Spełniamy wymóg 'required'
                Room = room    // <--- Spełniamy wymóg 'required'
            };

            _context.Showings.Add(showing);
            await _context.SaveChangesAsync();

            // Zwracamy DTO
            var resultDto = new ShowingDto
            {
                Id = showing.Id,
                Date = showing.Date,
                End_Date = showing.End_Date,
                Movie_Id = movie.Id,
                MovieTitle = movie.Title,
                Room_Id = room.Id,
                RoomNumber = showing.Room.Number
            };

            return CreatedAtAction(nameof(GetById), new { id = showing.Id }, resultDto);
        }

        // 4. PUT: api/showings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShowingDto model)
        {
            var showing = await _context.Showings
                .Include(s => s.Movie) // Ważne, żeby mieć dostęp do czasu trwania filmu
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showing == null) return NotFound("Seans nie istnieje.");

            // Sprawdzamy czy zmieniono film (jeśli tak, musimy pobrać nowy, by przeliczyć czas)
            if (showing.Movie_Id != model.Movie_Id)
            {
                var movie = await _context.Movies.FindAsync(model.Movie_Id);
                if (movie == null) return BadRequest("Nowy film nie istnieje.");
                
                showing.Movie = movie;
                showing.Movie_Id = model.Movie_Id;
            }

            // Sprawdzamy czy zmieniono salę
            if (showing.Room_Id != model.Room_Id)
            {
                var room = await _context.Rooms.FindAsync(model.Room_Id);
                if (room == null) return BadRequest("Nowa sala nie istnieje.");
                
                showing.Room = room;
                showing.Room_Id = model.Room_Id;
            }

            // Aktualizujemy datę
            showing.Date = model.Date;
            
            // Zawsze przeliczamy End_Date (bo mogła zmienić się data startu LUB film)
            // showing.Movie jest dostępny dzięki .Include lub powyższemu pobraniu
            showing.End_Date = showing.Date.AddMinutes(showing.Movie.Duration);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 5. DELETE: api/showings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var showing = await _context.Showings.FindAsync(id);
            if (showing == null) return NotFound("Seans nie istnieje.");

            _context.Showings.Remove(showing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}