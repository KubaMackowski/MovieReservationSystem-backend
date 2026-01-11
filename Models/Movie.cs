using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Potrzebne do atrybutu [ForeignKey]

namespace MovieReservationSystem.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Relase_Date { get; set; }
        public int Duration { get; set; }
        public required string Cast { get; set; }
        public required string Director { get; set; }
        public required string Production { get; set; }

        public required ICollection<MovieGenre> MovieGenres { get; set; }
        public ICollection<Showing>? Showings { get; set; }

        // --- RELACJA DO PLIKU (POSTER) ---

        // 1. Klucz obcy (nullable 'int?', jeśli plakat nie jest obowiązkowy przy tworzeniu filmu)
        public int? PosterId { get; set; }

        // 2. Właściwość nawigacyjna
        [ForeignKey("PosterId")]
        public File? Poster { get; set; } 
    }
}