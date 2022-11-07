namespace Business.DataTransferObjects;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Body { get; set; } = default!;
    public DateTimeOffset CreationDate { get; set; }
    
    public Guid? ParentId { get; set; }

    public CommentUserInfoDto UserInfo { get; set; } = default!;
}