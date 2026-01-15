using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.DTOs
{
    
    public class ReservationDto
    {
        public int Id { get; set; }
        public DateTime Created_At { get; set; }
        
       
        public string UserId { get; set; }
        public string UserEmail { get; set; } 

        
        public int ShowingId { get; set; }
        public string MovieTitle { get; set; }
        public DateTime ShowingDate { get; set; }

        
        public int SeatId { get; set; }
        public int SeatRow { get; set; }     
        public int SeatNumber { get; set; }  
    }

    
    public class CreateReservationDto
    {
        [Required]
        public string UserId { get; set; } 

        [Required]
        public int ShowingId { get; set; }

        [Required]
        public int SeatId { get; set; }
    }

    
    public class UpdateReservationDto
    {
        public int ShowingId { get; set; }
        public int SeatId { get; set; }
    }
}