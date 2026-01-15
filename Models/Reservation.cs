using Microsoft.AspNetCore.Identity; 
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieReservationSystem.Models 
{
    public class Reservation
    {
    public int Id { get; set; }
    public int User_Id { get; set; }
    public int Showing_Id { get; set; }
    public int Seat_Id { get; set; }
    public DateTime Created_At { get; set; }

    public string UserId { get; set; } 

    
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }    
    public required Showing Showing { get; set; }
    public required Seat Seat { get; set; }
   
 }
}