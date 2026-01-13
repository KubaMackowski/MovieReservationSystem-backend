
using Microsoft.AspNetCore.Identity; // <--- 1. DODANE
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // <--- 2. DODANE
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Models;

namespace MovieReservationSystem.Data
{
    // 3. ZMIANA: Dziedziczenie po IdentityDbContext<IdentityUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // UWAGA: IdentityDbContext sam zarządza użytkownikami w tabeli "AspNetUsers".
        // Jeśli Twoja klasa 'User' to stara tabela użytkowników, powinieneś ją usunąć 
        // lub zintegrować z IdentityUser. Na razie zostawiam, ale może powodować konflikt nazw.
        // public DbSet<User> Users { get; set; } 

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showing> Showings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Price> Prices { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
        {
            // 4. KLUCZOWE: Musisz wywołać konfigurację bazową Identity!
            // Bez tego Entity Framework nie wie, jak stworzyć tabele użytkowników.
            base.OnModelCreating(model); 

            // MovieGenres composite key
            model.Entity<MovieGenre>()
                .HasKey(mg => new { mg.Genre_Id, mg.Movie_Id });

            // One seat can't be reserved twice for same showing
            model.Entity<Reservation>()
                .HasIndex(r => new { r.Showing_Id, r.Seat_Id })
                .IsUnique();

            // Relationships
            model.Entity<Reservation>()
                .HasOne(r => r.User)         // Rezerwacja ma jednego Usera (typu ApplicationUser)
                .WithMany(u => u.Reservations) // Teraz to zadziała! Użytkownik ma wiele rezerwacji
                .HasForeignKey(r => r.UserId); // Upewnij się, że w modelu Reservation pole nazywa się UserId

            model.Entity<Reservation>()
                .HasOne(r => r.Seat)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.Seat_Id);

            model.Entity<Reservation>()
                .HasOne(r => r.Showing)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.Showing_Id);

            model.Entity<Seat>()
                .HasOne(s => s.Room)
                .WithMany(r => r.SeatsList)
                .HasForeignKey(s => s.Room_Id);

            model.Entity<Showing>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Showings)
                .HasForeignKey(s => s.Room_Id);

            model.Entity<Showing>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Showings)
                .HasForeignKey(s => s.Movie_Id);

            model.Entity<Price>()
                .HasOne(p => p.Showing)
                .WithMany(s => s.Prices)
                .HasForeignKey(p => p.Showing_Id);
        }
    }
}