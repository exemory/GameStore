namespace Business.DataTransferObjects;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public bool HasAvatar { get; set; }
}