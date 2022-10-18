namespace Business.DataTransferObjects
{
    public class SignInDto
    {
        public string Login { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}