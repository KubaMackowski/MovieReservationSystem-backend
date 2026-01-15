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
    [Authorize(Roles = "ADMIN")] // Tylko Admin zarządza salami
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        [AllowAnonymous] 
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
        {
            var rooms = await _context.Rooms
                .Include(r => r.SeatsList) 
                .OrderBy(r => r.Number)
                .ToListAsync();

            var dtos = rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Number = r.Number,
                Seats = r.Seats,
                GeneratedSeatsCount = r.SeatsList.Count, 
                SeatObjects = r.SeatsList.Select(s => new MSeatDto
                {
                    Id = s.Id,
                    Row = s.Row,
                    Number = s.Number
                }).ToArray()
            });

            return Ok(dtos);
        }

        
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RoomDto>> GetById(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.SeatsList)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return NotFound("Sala nie istnieje.");

            return Ok(new RoomDto
            {
                Id = room.Id,
                Number = room.Number,
                Seats = room.Seats,
                GeneratedSeatsCount = room.SeatsList.Count,
                SeatObjects = room.SeatsList.Select(s => new MSeatDto
                {
                    Id = s.Id,
                    Row = s.Row,
                    Number = s.Number
                }).ToArray()
            });
        }

        
        [HttpPost]
        public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            
            if (await _context.Rooms.AnyAsync(r => r.Number == model.Number))
            {
                return BadRequest($"Sala o numerze {model.Number} już istnieje.");
            }

            var room = new Room
            {
                Number = model.Number,
                Seats = model.Seats,
                SeatsList = new List<Seat>() 
            };

            
            
            
            
            int seatsPerRow = 10;
            for (int i = 0; i < model.Seats; i++)
            {
                int row = (i / seatsPerRow) + 1;
                int number = (i % seatsPerRow) + 1;

                room.SeatsList.Add(new Seat
                {
                    Row = row,
                    Number = number,
                    Room = room 
                });
            }
            

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var dto = new RoomDto
            {
                Id = room.Id,
                Number = room.Number,
                Seats = room.Seats,
                GeneratedSeatsCount = room.SeatsList.Count,
                SeatObjects = room.SeatsList.Select(s => new MSeatDto
                {
                    Id = s.Id,
                    Row = s.Row,
                    Number = s.Number
                }).ToArray()
            };

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, dto);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto model)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound("Sala nie istnieje.");

            
            if (room.Number != model.Number && await _context.Rooms.AnyAsync(r => r.Number == model.Number))
            {
                return BadRequest($"Sala o numerze {model.Number} już istnieje.");
            }

            room.Number = model.Number;
            
            
            room.Seats = model.Seats;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound("Sala nie istnieje.");

            

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}