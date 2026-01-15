using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.DTOs
{
    
    public class ShowingDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime End_Date { get; set; }
        
        
        public int Movie_Id { get; set; }
        public string MovieTitle { get; set; }
        
        public int Room_Id { get; set; }
        public int RoomNumber { get; set; } 
    }

    
    public class CreateShowingDto
    {
        [Required]
        public int Movie_Id { get; set; }

        [Required]
        public int Room_Id { get; set; }

        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public decimal Price { get; set; }
    }

    
    public class UpdateShowingDto
    {
        public int Movie_Id { get; set; }
        public int Room_Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }
}