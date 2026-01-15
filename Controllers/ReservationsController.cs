using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Data;
using MovieReservationSystem.DTOs;
using MovieReservationSystem.Models;
using System.Security.Claims; 

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        [Authorize(Roles = "ADMIN")] 
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetAll()
        {
            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Showing)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.Seat)
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
                SeatRow = r.Seat.Row,
                SeatNumber = r.Seat.Number
            });

            return Ok(dtos);
        }

        
        [HttpGet("my")]
        [Authorize] 
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetMyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var reservations = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Showing)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.Seat)
                .Where(r => r.UserId == userId) 
                .OrderByDescending(r => r.Created_At)
                .ToListAsync();

            var dtos = reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                Created_At = r.Created_At,
                UserId = r.UserId,
                UserEmail = r.User?.Email,
                ShowingId = r.Showing_Id,
                MovieTitle = r.Showing?.Movie?.Title,
                ShowingDate = r.Showing?.Date ?? DateTime.MinValue,
                SeatId = r.Seat_Id,
                SeatRow = r.Seat?.Row ?? 0,
                SeatNumber = r.Seat?.Number ?? 0
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        [Authorize] 
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

        [HttpPost]
        [Authorize] 
        public async Task<ActionResult<ReservationDto>> Create([FromBody] CreateReservationDto model)
        {
            
            
            if (!ModelState.IsValid) return BadRequest(ModelState);

            bool isSeatTaken = await _context.Reservations.AnyAsync(r => 
                r.Showing_Id == model.ShowingId && r.Seat_Id == model.SeatId);

            if (isSeatTaken)
            {
                return Conflict("To miejsce jest już zarezerwowane na ten seans.");
            }

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null) return BadRequest("Użytkownik nie istnieje.");

            var showing = await _context.Showings.Include(s => s.Movie).FirstOrDefaultAsync(s => s.Id == model.ShowingId);
            if (showing == null) return BadRequest("Seans nie istnieje.");

            var seat = await _context.Seats.FindAsync(model.SeatId);
            if (seat == null) return BadRequest("Miejsce nie istnieje.");

            var reservation = new Reservation
            {
                Created_At = DateTime.UtcNow,
                UserId = model.UserId,
                User = user,       
                Showing_Id = model.ShowingId,
                Showing = showing, 
                Seat_Id = model.SeatId,
                Seat = seat        
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

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
        
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")] 
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReservationDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound("Rezerwacja nie istnieje.");

            reservation.Showing_Id = model.ShowingId;
            reservation.Seat_Id = model.SeatId;

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")] 
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