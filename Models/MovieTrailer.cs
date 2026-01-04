namespace MovieReservationSystem.Models // <--- DODAJ
{
    public class MovieTrailer
    {
    public int Id { get; set; }
    public int Trailer_Id { get; set; }
    public int Movie_Id { get; set; }

    public required Trailer Trailer { get; set; }
    public required Movie Movie { get; set; }
}
}