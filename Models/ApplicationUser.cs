using Microsoft.AspNetCore.Identity;

namespace MovieReservationSystem.Models;

public class ApplicationUser : IdentityUser
{
    
    public ICollection<Reservation> Reservations { get; set; }
}