namespace Data.Entities;

public class Comment : EntityBase
{
    public string Body { get; set; } = default!;
    public DateTimeOffset CreationDate { get; set; }
    public bool Deleted { get; set; }

    public Guid? ParentId { get; set; }
    public Comment? Parent { get; set; }

    public Guid GameId { get; set; }
    public Game Game { get; set; } = default!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
}