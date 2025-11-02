// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
// using MovieReservationSystem.Models; // Upewnij się, że ta przestrzeń nazw jest poprawna

namespace MovieReservationSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Ta opcja jest potrzebna do skonfigurowania połączenia w Program.cs
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}