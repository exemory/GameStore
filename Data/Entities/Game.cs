namespace Data.Entities;

public class Game : EntityBase
{
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string Description { get; set; } = default!;

    public IEnumerable<Comment> Comments { get; set; } = default!;
    public IEnumerable<Genre> Genres { get; set; } = default!;
    public IEnumerable<PlatformType> PlatformTypes { get; set; } = default!;
}