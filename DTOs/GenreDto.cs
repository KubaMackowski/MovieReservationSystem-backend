using System.ComponentModel.DataAnnotations;
namespace MovieReservationSystem.DTOs
{
    // Do wyświetlania danych
    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // Do tworzenia (ID tworzy baza)
    public class CreateGenreDto
    {
        [Required(ErrorMessage = "Nazwa gatunku jest wymagana.")]
        public string Name { get; set; }
    }

    // Do edycji
    public class UpdateGenreDto
    {
        [Required(ErrorMessage = "Nazwa gatunku jest wymagana.")]
        public string Name { get; set; }
    }
}