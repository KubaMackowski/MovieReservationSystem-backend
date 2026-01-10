using Microsoft.AspNetCore.Identity;

namespace MovieReservationSystem.Models;

public class ApplicationUser : IdentityUser
{
    // Dodajesz relację, której brakuje w czystym IdentityUser
    public ICollection<Reservation> Reservations { get; set; }
}