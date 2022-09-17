namespace Business.DataTransferObjects;

public class GameWithDetailsDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public IEnumerable<string> Genres { get; set; } = default!;
    public IEnumerable<string> PlatformTypes { get; set; } = default!;
}