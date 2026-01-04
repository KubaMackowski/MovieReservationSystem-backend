using System.Collections.Generic;

namespace MovieReservationSystem.Models // <--- Pamiętaj o namespace!
{
public class Trailer
{
    public int Id { get; set; }
    public required string Source { get; set; }
    public required string Thumbnail { get; set; }

    public ICollection<MovieTrailer>? MovieTrailers { get; set; }
}
}