using System.Collections.Generic;

namespace MovieReservationSystem.Models // <--- 1. Upewnij się, że masz ten namespace!
{
    public class Movie
    {
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Status { get; set; }
    public DateTime Created_At { get; set; }
    public DateTime Relase_Date { get; set; }
    public int Duration { get; set; }
    public required string Cast { get; set; }
    public required string Director { get; set; }
    public required string Production { get; set; }

    public required ICollection<MovieGenre> MovieGenres { get; set; }
    public  ICollection<MovieTrailer>? MovieTrailers { get; set; }
    public  ICollection<Showing>? Showings { get; set; }
    public required ICollection<Photo> Photos { get; set; }
}
}