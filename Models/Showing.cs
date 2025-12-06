public class Showing
{
    public int Id { get; set; }
    public int Movie_Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime End_Date { get; set; }
    public int Room_Id { get; set; }

    public required Movie Movie { get; set; }
    public required Room Room { get; set; }
    public ICollection<Reservation>? Reservations { get; set; }
    public ICollection<Price>? Prices { get; set; }
}