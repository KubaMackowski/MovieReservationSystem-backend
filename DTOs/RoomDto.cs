using System.ComponentModel.DataAnnotations;
using MovieReservationSystem.Models;

namespace MovieReservationSystem.DTOs
{
    // 1. Wyświetlanie (READ)
    public class RoomDto
    {
        public int Id { get; set; }
        public int Number { get; set; } // Numer sali
        public int Seats { get; set; }  // Pojemność (liczba miejsc)
        
        // Opcjonalnie: informacja ile fizycznie jest miejsc w liście
        public int GeneratedSeatsCount { get; set; } 
        
        public required MSeatDto[] SeatObjects { get; set; }
    }

    // 2. Tworzenie (CREATE)
    public class CreateRoomDto
    {
        [Required(ErrorMessage = "Numer sali jest wymagany")]
        [Range(1, 100, ErrorMessage = "Numer sali musi być między 1 a 100")]
        public int Number { get; set; }

        [Required]
        [Range(1, 500, ErrorMessage = "Sala musi mieć od 1 do 500 miejsc")]
        public int Seats { get; set; } // Oczekiwana liczba miejsc
    }

    // 3. Aktualizacja (UPDATE)
    public class UpdateRoomDto
    {
        public int Number { get; set; }
        public int Seats { get; set; }
    }
}