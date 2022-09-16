namespace Data.Entities;

public class Comment : EntityBase
{
    public string Name { get; set; } = default!;
    public string Body { get; set; } = default!;

    public Guid? ParentId { get; set; }
    public Comment? Parent { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; } = default!;
}