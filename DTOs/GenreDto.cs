using System.ComponentModel.DataAnnotations;
namespace MovieReservationSystem.DTOs
{
    
    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    
    public class CreateGenreDto
    {
        [Required(ErrorMessage = "Nazwa gatunku jest wymagana.")]
        public string Name { get; set; }
    }

    
    public class UpdateGenreDto
    {
        [Required(ErrorMessage = "Nazwa gatunku jest wymagana.")]
        public string Name { get; set; }
    }
}