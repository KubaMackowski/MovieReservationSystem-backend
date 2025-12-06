public class Room
{
    public int Id { get; set; }
    public int Number { get; set; }
    public int Seats { get; set; }

    public required ICollection<Seat> SeatsList { get; set; }
    public ICollection<Showing>? Showings { get; set; }
}