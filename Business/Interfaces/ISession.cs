namespace Business.Interfaces
{
    /// <summary>
    /// Represents request session
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Checks whether the user is authorized or not
        /// </summary>
        public bool IsAuthorized { get; }
        
        /// <summary>
        /// Guid of the authorized user, if authorized, null otherwise
        /// </summary>
        public Guid? UserId { get; }
        
        /// <summary>
        /// Roles of the authorized user, if authorized, null otherwise
        /// </summary>
        public IEnumerable<string> UserRoles { get; }

        /// <summary>
        /// Initializes request session
        /// </summary>
        /// <param name="userId">Guid of the authorized user</param>
        /// <param name="userRoles">Roles of the authorized user</param>
        public void Initialize(Guid userId, IEnumerable<string> userRoles);
    }
}