namespace Business.DataTransferObjects;

public class GenreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid? ParentId { get; set; }
}