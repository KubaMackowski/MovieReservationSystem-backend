using System.ComponentModel.DataAnnotations;
using MovieReservationSystem.Models;

namespace MovieReservationSystem.DTOs
{
    
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Relase_Date { get; set; } 
        public int Duration { get; set; }
        public string Director { get; set; }
        public string Production { get; set; }
        public string Cast { get; set; }
        public string Poster { get; set; }
        public string PosterPath { get; set; }
        
        
        public List<string> Genres { get; set; } = new List<string>();
        public List<MShowingDto> Showings { get; set; } = new List<MShowingDto>();
    }
    
    public class MShowingDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime End_Date { get; set; }
        public decimal Price { get; set; }
        public MRoomDto Room { get; set; } = null!;
    }

    public class MRoomDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public List<MSeatDto> Seats { get; set; } = new();
    }

    public class MSeatDto
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public int Number { get; set; }
        public bool IsOccupied { get; set; } 
    }

    
    public class CreateMovieDto
    {
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public string Status { get; set; } 
        
        [Required]
        public DateTime Relase_Date { get; set; }
        
        [Range(1, 1000)]
        public int Duration { get; set; } 
        
        [Required]
        public string Director { get; set; }
        
        [Required]
        public string Production { get; set; }
        
        [Required]
        public string Cast { get; set; }
        
        public string PosterPath { get; set; }

        
        public List<int> GenreIds { get; set; } = new List<int>();
    }

   
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
        public string PosterPath { get; set; }
        
        
        public List<int>? GenreIds { get; set; }
    }
}