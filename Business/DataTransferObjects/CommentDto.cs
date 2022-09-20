namespace Business.DataTransferObjects;

public class CommentDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Body { get; set; }

    public Guid? ParentId { get; set; }
}