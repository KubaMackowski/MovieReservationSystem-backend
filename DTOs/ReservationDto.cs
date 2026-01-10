using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.DTOs
{
    // 1. Wyświetlanie (READ)
    public class ReservationDto
    {
        public int Id { get; set; }
        public DateTime Created_At { get; set; }
        
        // Dane Użytkownika
        public string UserId { get; set; }
        public string UserEmail { get; set; } // Przydatne dla admina

        // Dane Seansu
        public int ShowingId { get; set; }
        public string MovieTitle { get; set; }
        public DateTime ShowingDate { get; set; }

        // Dane Miejsca
        public int SeatId { get; set; }
        public int SeatRow { get; set; }     // Zakładam, że model Seat ma pole Row
        public int SeatNumber { get; set; }  // Zakładam, że model Seat ma pole Number
    }

    // 2. Tworzenie (CREATE)
    public class CreateReservationDto
    {
        [Required]
        public string UserId { get; set; } // Admin przypisuje rezerwację do usera

        [Required]
        public int ShowingId { get; set; }

        [Required]
        public int SeatId { get; set; }
    }

    // 3. Aktualizacja (UPDATE) - np. zmiana miejsca
    public class UpdateReservationDto
    {
        public int ShowingId { get; set; }
        public int SeatId { get; set; }
    }
}