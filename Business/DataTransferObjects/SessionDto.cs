namespace Business.DataTransferObjects
{
    public class SessionDto
    {
        public UserInfoDto UserInfo { get; set; } = default!;
        public string AccessToken { get; set; } = default!;
    }
}