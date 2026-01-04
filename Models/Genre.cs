using System.Collections.Generic;

namespace MovieReservationSystem.Models // <--- Pamiętaj o namespace!
{
public class Genre

{
    public int Id { get; set; }
    public required string Name { get; set; }

    public  ICollection<MovieGenre>? MovieGenres { get; set; }
}
}