public class User
{
    public int Id { get; set; }
    public required string First_Name { get; set; }
    public required string Last_Name { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public DateTime Created_At { get; set; }

    public ICollection<Reservation>? Reservations { get; set; }
}