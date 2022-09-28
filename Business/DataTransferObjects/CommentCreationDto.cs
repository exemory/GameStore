namespace Business.DataTransferObjects;

public class CommentCreationDto
{
    public Guid GameId { get; set; }
    public string Name { get; set; } = default!;
    public string Body { get; set; } = default!;
    public Guid? ParentId { get; set; }
}