namespace Business.DataTransferObjects;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Body { get; set; } = default!;

    public Guid? ParentId { get; set; }
}