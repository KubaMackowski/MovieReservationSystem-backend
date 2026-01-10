using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.DTOs
{
    // 1. Wyświetlanie (READ)
    public class ShowingDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime End_Date { get; set; }
        
        // Zwracamy informacje o filmie i sali, żeby frontend wiedział co to za seans
        public int Movie_Id { get; set; }
        public string MovieTitle { get; set; }
        
        public int Room_Id { get; set; }
        public int RoomNumber { get; set; } // Zakładam, że model Room ma pole Name lub Number
    }

    // 2. Tworzenie (CREATE)
    public class CreateShowingDto
    {
        [Required]
        public int Movie_Id { get; set; }

        [Required]
        public int Room_Id { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }

    // 3. Aktualizacja (UPDATE)
    public class UpdateShowingDto
    {
        public int Movie_Id { get; set; }
        public int Room_Id { get; set; }
        public DateTime Date { get; set; }
    }
}