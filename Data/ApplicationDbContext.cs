
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Models;

namespace MovieReservationSystem.Data
{
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showing> Showings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
        {
            
            base.OnModelCreating(model); 

            
            model.Entity<MovieGenre>()
                .HasKey(mg => new { mg.Genre_Id, mg.Movie_Id });

            
            model.Entity<Reservation>()
                .HasIndex(r => new { r.Showing_Id, r.Seat_Id })
                .IsUnique();

            
            model.Entity<Reservation>()
                .HasOne(r => r.User)         
                .WithMany(u => u.Reservations) 
                .HasForeignKey(r => r.UserId); 

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
        }
    }
}