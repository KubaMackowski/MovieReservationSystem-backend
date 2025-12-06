public class File
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Format { get; set; }
    public required string File_Name { get; set; }
    public required string Hash { get; set; }

    public ICollection<Photo>? Photos { get; set; }
}
