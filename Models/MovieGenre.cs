public class MovieGenre
{
    public int Genre_Id { get; set; }
    public int Movie_Id { get; set; }

    public required Movie Movie { get; set; }
    public required Genre Genre { get; set; }
}