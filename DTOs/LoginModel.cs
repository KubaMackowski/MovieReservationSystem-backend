using System.ComponentModel.DataAnnotations;
namespace MovieReservationSystem.DTOs
{
    

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
}