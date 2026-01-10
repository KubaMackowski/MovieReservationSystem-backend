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

        // 1. GET: api/rooms
        [HttpGet]
        [AllowAnonymous] // Użytkownik może chcieć zobaczyć jakie są sale
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
        {
            var rooms = await _context.Rooms
                .Include(r => r.SeatsList) // Dołączamy listę, żeby policzyć miejsca
                .OrderBy(r => r.Number)
                .ToListAsync();

            var dtos = rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Number = r.Number,
                Seats = r.Seats,
                GeneratedSeatsCount = r.SeatsList.Count // Ile faktycznie jest obiektów Seat
            });

            return Ok(dtos);
        }

        // 2. GET: api/rooms/5
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
                GeneratedSeatsCount = room.SeatsList.Count
            });
        }

        // 3. POST: api/rooms
        [HttpPost]
        public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Sprawdź, czy sala o takim numerze już istnieje
            if (await _context.Rooms.AnyAsync(r => r.Number == model.Number))
            {
                return BadRequest($"Sala o numerze {model.Number} już istnieje.");
            }

            var room = new Room
            {
                Number = model.Number,
                Seats = model.Seats,
                SeatsList = new List<Seat>() // Inicjalizacja pustej listy
            };

            // --- AUTOMATYCZNE GENEROWANIE MIEJSC (Opcjonalne) ---
            // To jest bardzo przydatne, bo bez obiektów Seat nie zrobisz rezerwacji.
            // Zakładamy prosty układ: 10 miejsc w rzędzie.
            
            int seatsPerRow = 10;
            for (int i = 0; i < model.Seats; i++)
            {
                int row = (i / seatsPerRow) + 1;
                int number = (i % seatsPerRow) + 1;

                room.SeatsList.Add(new Seat
                {
                    Row = row,
                    Number = number,
                    Room = room // EF Core sam ogarnie powiązanie
                });
            }
            // -----------------------------------------------------

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var dto = new RoomDto
            {
                Id = room.Id,
                Number = room.Number,
                Seats = room.Seats,
                GeneratedSeatsCount = room.SeatsList.Count
            };

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, dto);
        }

        // 4. PUT: api/rooms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto model)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound("Sala nie istnieje.");

            // Sprawdź czy numer nie jest zajęty przez inną salę
            if (room.Number != model.Number && await _context.Rooms.AnyAsync(r => r.Number == model.Number))
            {
                return BadRequest($"Sala o numerze {model.Number} już istnieje.");
            }

            room.Number = model.Number;
            
            // Uwaga: Zmiana liczby miejsc ('Seats') tutaj nie usuwa/dodaje 
            // fizycznych obiektów Seat z listy SeatsList. 
            // To wymagałoby bardziej skomplikowanej logiki (usuwanie rezerwacji itp.).
            // Tutaj aktualizujemy tylko liczbę informacyjną.
            room.Seats = model.Seats;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 5. DELETE: api/rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound("Sala nie istnieje.");

            // Opcjonalnie: Zablokuj usunięcie, jeśli są przyszłe seanse w tej sali
            // var hasShowings = await _context.Showings.AnyAsync(s => s.Room_Id == id && s.Date > DateTime.UtcNow);
            // if (hasShowings) return BadRequest("Nie można usunąć sali, która ma zaplanowane przyszłe seanse.");

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}