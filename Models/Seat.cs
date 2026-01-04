namespace MovieReservationSystem.Models
{
public class Seat
{
    public int Id { get; set; }
    public int Row { get; set; }
    public int Number { get; set; }
    public int Room_Id { get; set; }

    public required Room Room { get; set; }
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
}