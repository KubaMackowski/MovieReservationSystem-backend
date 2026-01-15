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
    
    public class ShowingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShowingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ShowingDto>>> GetAll()
        {
            var showings = await _context.Showings
                .Include(s => s.Movie) 
                .Include(s => s.Room)  
                .OrderBy(s => s.Date)  
                .ToListAsync();

           var showingDtos = showings.Select(s => new ShowingDto
                {
                    Id = s.Id,
                    Date = s.Date,
                    End_Date = s.End_Date,
                    Movie_Id = s.Movie_Id,
                    MovieTitle = s.Movie.Title,
                    Room_Id = s.Room_Id,
                    RoomNumber = s.Room.Number 
                });

            return Ok(showingDtos);
        }

        
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
                RoomNumber = showing.Room.Number 
            });
        }
        
        [HttpPost]
        public async Task<ActionResult<ShowingDto>> Create([FromBody] CreateShowingDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            
            var movie = await _context.Movies.FindAsync(model.Movie_Id);
            if (movie == null) return BadRequest("Podany film nie istnieje.");

            
            var room = await _context.Rooms.FindAsync(model.Room_Id);
            if (room == null) return BadRequest("Podana sala nie istnieje.");

            
            var endDate = model.Date.AddMinutes(movie.Duration);

            

            
            var showing = new Showing
            {
                Date = model.Date,
                End_Date = endDate,
                Movie_Id = model.Movie_Id,
                Room_Id = model.Room_Id,
                Movie = movie, 
                Room = room,    
                Price = model.Price,
            };

            _context.Showings.Add(showing);
            await _context.SaveChangesAsync();

            
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

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateShowingDto model)
        {
            var showing = await _context.Showings
                .Include(s => s.Movie) 
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (showing == null) return NotFound("Seans nie istnieje.");

            
            if (showing.Movie_Id != model.Movie_Id)
            {
                var movie = await _context.Movies.FindAsync(model.Movie_Id);
                if (movie == null) return BadRequest("Nowy film nie istnieje.");
                
                showing.Movie = movie;
                showing.Movie_Id = model.Movie_Id;
            }

            
            if (showing.Room_Id != model.Room_Id)
            {
                var room = await _context.Rooms.FindAsync(model.Room_Id);
                if (room == null) return BadRequest("Nowa sala nie istnieje.");
                
                showing.Room = room;
                showing.Room_Id = model.Room_Id;
            }

            
            showing.Date = model.Date;
            showing.Price = model.Price;
            
            
            showing.End_Date = showing.Date.AddMinutes(showing.Movie.Duration);

            await _context.SaveChangesAsync();
            return NoContent();
        }

       
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