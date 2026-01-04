using System.ComponentModel.DataAnnotations; // Opcjonalne, jeśli używasz adnotacji
using MovieReservationSystem.Models; // To dla pewności, choć jesteśmy w tym namespace

namespace MovieReservationSystem.Models // <--- TO JEST KLUCZOWE!
{
    public class User
    {
        public int Id { get; set; }
        public required string First_Name { get; set; }
        public required string Last_Name { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public DateTime Created_At { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        // Pamiętaj, że Reservation musi być też widoczne (dodaj using jeśli jest w innym folderze)
        // public ICollection<Reservation>? Reservations { get; set; } 
    }
}