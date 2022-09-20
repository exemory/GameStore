namespace Business.DataTransferObjects;

public class CommentCreationDto
{
    public string? Name { get; set; }
    public string? Body { get; set; }
    public Guid? ParentId { get; set; }
}