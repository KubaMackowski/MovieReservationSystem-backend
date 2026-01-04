using System.Collections.Generic;

namespace MovieReservationSystem.Models // <--- Pamiętaj o namespace!
{
    public class Photo
    {
    public int Id { get; set; }
    public int Movie_Id { get; set; }
    public int File_Id { get; set; }
    public int Order { get; set; }

    public required Movie Movie { get; set; }
    public required File File { get; set; }
}
}