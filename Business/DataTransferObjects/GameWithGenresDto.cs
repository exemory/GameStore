namespace Business.DataTransferObjects;

public class GameWithGenresDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string Description { get; set; } = default!;
    
    public IEnumerable<string> Genres { get; set; } = new List<string>();
}