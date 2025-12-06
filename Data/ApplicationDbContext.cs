using Microsoft.EntityFrameworkCore;
namespace MovieReservationSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showing> Showings { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Trailer> Trailers { get; set; }
        public DbSet<MovieTrailer> MovieTrailers { get; set; }
        public DbSet<Price> Prices { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
        {
            // MovieGenres composite key
            model.Entity<MovieGenre>()
                .HasKey(mg => new { mg.Genre_Id, mg.Movie_Id });

            // One seat can't be reserved twice for same showing
            model.Entity<Reservation>()
                .HasIndex(r => new { r.Showing_Id, r.Seat_Id })
                .IsUnique();

            // Relationships
            model.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.User_Id);

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

            model.Entity<MovieTrailer>()
                .HasOne(mt => mt.Movie)
                .WithMany(m => m.MovieTrailers)
                .HasForeignKey(mt => mt.Movie_Id);

            model.Entity<MovieTrailer>()
                .HasOne(mt => mt.Trailer)
                .WithMany(t => t.MovieTrailers)
                .HasForeignKey(mt => mt.Trailer_Id);

            model.Entity<Photo>()
                .HasOne(p => p.Movie)
                .WithMany(m => m.Photos)
                .HasForeignKey(p => p.Movie_Id);

            model.Entity<Photo>()
                .HasOne(p => p.File)
                .WithMany(f => f.Photos)
                .HasForeignKey(p => p.File_Id);

            model.Entity<Price>()
                .HasOne(p => p.Showing)
                .WithMany(s => s.Prices)
                .HasForeignKey(p => p.Showing_Id);
        }
    }
}