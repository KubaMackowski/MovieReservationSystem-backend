using System.Collections.Generic;

namespace MovieReservationSystem.Models 
{
    public class Room
    {
    public int Id { get; set; }
    public int Number { get; set; }
    public int Seats { get; set; }

    public ICollection<Seat> SeatsList { get; set; } = new List<Seat>();
    public ICollection<Showing>? Showings { get; set; }
}
}