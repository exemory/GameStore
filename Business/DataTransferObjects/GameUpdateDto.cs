namespace Business.DataTransferObjects;

public class GameUpdateDto
{
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string Description { get; set; } = default!;
    public string? ImageFileName { get; set; } = default!;
    
    public ICollection<Guid>? GenreIds { get; set; }
    public ICollection<Guid>? PlatformTypeIds { get; set; }
}