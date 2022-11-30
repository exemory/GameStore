using Data.Entities;

namespace Data.Interfaces
{
    /// <summary>
    /// Repository of <see cref="User"/> entities
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>
        /// List of application users
        /// </returns>
        public Task<IEnumerable<User>> GetAllAsync();
    }
}