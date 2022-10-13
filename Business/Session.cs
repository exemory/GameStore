using Business.Interfaces;

namespace Business
{
    /// <inheritdoc />
    public class Session : ISession
    {
        public Guid? UserId { get; private set; }
        public IEnumerable<string> UserRoles { get; private set; } = new List<string>();
        public bool IsAuthorized => UserId != null;

        public void Initialize(Guid userId, IEnumerable<string> userRoles)
        {
            UserId = userId;
            UserRoles = userRoles;
        }
    }
}