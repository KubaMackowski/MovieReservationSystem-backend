using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.DTOs
{
    // 1. DTO do wyświetlania (READ)
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Relase_Date { get; set; } // Zachowałem Twoją nazwę z literówką
        public int Duration { get; set; }
        public string Director { get; set; }
        public string Production { get; set; }
        public string Cast { get; set; }
        public string Poster { get; set; }
        
        // Zwracamy listę nazw gatunków, zamiast skomplikowanych obiektów
        public List<string> Genres { get; set; } = new List<string>();
    }

    // 2. DTO do tworzenia (CREATE)
    public class CreateMovieDto
    {
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public string Status { get; set; } // Np. "Available", "Coming Soon"
        
        [Required]
        public DateTime Relase_Date { get; set; }
        
        [Range(1, 1000)]
        public int Duration { get; set; } // W minutach
        
        [Required]
        public string Director { get; set; }
        
        [Required]
        public string Production { get; set; }
        
        [Required]
        public string Cast { get; set; }

        // Lista ID gatunków, które chcemy przypisać do filmu (np. [1, 2])
        public List<int> GenreIds { get; set; } = new List<int>();
    }

    // 3. DTO do aktualizacji (UPDATE)
    public class UpdateMovieDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Relase_Date { get; set; }
        public int Duration { get; set; }
        public string Director { get; set; }
        public string Production { get; set; }
        public string Cast { get; set; }
        
        // Opcjonalnie: Aktualizacja listy gatunków
        public List<int>? GenreIds { get; set; }
    }
}