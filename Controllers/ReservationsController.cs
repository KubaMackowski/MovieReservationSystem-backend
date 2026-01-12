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
    [Authorize(Roles = "ADMIN")] // Tylko admin zarządza rezerwacjami w tym kontrolerze
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAll()
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)           // Dołącz Usera
                .Include(r => r.Showing)        // Dołącz Seans
                    .ThenInclude(s => s.Movie)  // ... i Film w seansie
                .Include(r => r.Seat)           // Dołącz Miejsce
                .OrderByDescending(r => r.Created_At)
                .ToListAsync();

            var dtos = reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                Created_At = r.Created_At,
                UserId = r.UserId,
                UserEmail = r.User.Email,
                ShowingId = r.Showing_Id,
                MovieTitle = r.Showing.Movie.Title,
                ShowingDate = r.Showing.Date,
                SeatId = r.Seat_Id,
                SeatRow = r.Seat.Row,       // Upewnij się, że masz takie pole w Seat
                SeatNumber = r.Seat.Number  // Upewnij się, że masz takie pole w Seat
            });

            return Ok(dtos);
        }

        // 2. GET: api/reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetById(int id)
        {
            var r = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Showing)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.Seat)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (r == null) return NotFound("Rezerwacja nie istnieje.");

            return Ok(new ReservationDto
            {
                Id = r.Id,
                Created_At = r.Created_At,
                UserId = r.UserId,
                UserEmail = r.User.Email,
                ShowingId = r.Showing_Id,
                MovieTitle = r.Showing.Movie.Title,
                ShowingDate = r.Showing.Date,
                SeatId = r.Seat_Id,
                SeatRow = r.Seat.Row,
                SeatNumber = r.Seat.Number
            });
        }

        // 3. POST: api/reservations
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ReservationDto>> Create([FromBody] CreateReservationDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // A. Walidacja czy miejsce jest już zajęte na ten seans
            bool isSeatTaken = await _context.Reservations.AnyAsync(r => 
                r.Showing_Id == model.ShowingId && r.Seat_Id == model.SeatId);

            if (isSeatTaken)
            {
                return Conflict("To miejsce jest już zarezerwowane na ten seans.");
            }

            // B. Pobieranie obiektów (wymagane przez 'required' w modelu)
            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null) return BadRequest("Użytkownik nie istnieje.");

            var showing = await _context.Showings.Include(s => s.Movie).FirstOrDefaultAsync(s => s.Id == model.ShowingId);
            if (showing == null) return BadRequest("Seans nie istnieje.");

            var seat = await _context.Seats.FindAsync(model.SeatId);
            if (seat == null) return BadRequest("Miejsce nie istnieje.");

            // C. Tworzenie rezerwacji
            var reservation = new Reservation
            {
                Created_At = DateTime.UtcNow,
                UserId = model.UserId,
                User = user,       // <--- Spełniamy wymóg 'required'
                Showing_Id = model.ShowingId,
                Showing = showing, // <--- Spełniamy wymóg 'required'
                Seat_Id = model.SeatId,
                Seat = seat        // <--- Spełniamy wymóg 'required'
                // Ignoruję User_Id (int), bo używamy string UserId dla Identity
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Zwracamy DTO
            var dto = new ReservationDto
            {
                Id = reservation.Id,
                Created_At = reservation.Created_At,
                UserId = user.Id,
                UserEmail = user.Email,
                ShowingId = showing.Id,
                MovieTitle = showing.Movie.Title,
                ShowingDate = showing.Date,
                SeatId = seat.Id,
                SeatRow = seat.Row,
                SeatNumber = seat.Number
            };

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, dto);
        }

        // 4. DELETE: api/reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound("Rezerwacja nie istnieje.");

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}