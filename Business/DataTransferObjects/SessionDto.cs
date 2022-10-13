namespace Business.DataTransferObjects
{
    public class SessionDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public IEnumerable<string> UserRoles { get; set; } = new List<string>();
        public string AccessToken { get; set; } = default!;
    }
}