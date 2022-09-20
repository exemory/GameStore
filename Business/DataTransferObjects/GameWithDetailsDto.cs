namespace Business.DataTransferObjects;

public class GameWithDetailsDto
{
    public Guid Id { get; set; }
    public string? Key { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> Genres { get; set; } = new List<string>();
    public IEnumerable<string> PlatformTypes { get; set; } = new List<string>();
}