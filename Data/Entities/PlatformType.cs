namespace Data.Entities;

public class PlatformType : EntityBase
{
    public string Type { get; set; } = default!;

    public IEnumerable<Game> Games { get; set; } = default!;
}