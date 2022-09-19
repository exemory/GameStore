namespace Business.DataTransferObjects;

public class CommentCreationDto
{
    public string Name { get; set; } = default!;
    public string Body { get; set; } = default!;
    public Guid? ParentId { get; set; }
}