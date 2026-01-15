using System.Collections.Generic; 

namespace MovieReservationSystem.Models
{
public class Showing
{


    public int Id { get; set; }
    public int Movie_Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime End_Date { get; set; }
    public int Room_Id { get; set; }
    public required Movie Movie { get; set; }
    public required Room Room { get; set; }
    public required decimal Price { get; set; }
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
}