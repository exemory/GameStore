using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <inheritdoc />
    public class UserRepository : IUserRepository
    {
        private readonly DbSet<User> _entities;

        /// <summary>
        /// Constructor for initializing a <see cref="UserRepository"/> class instance
        /// </summary>
        /// <param name="context">Context of the database</param>
        public UserRepository(GameStoreContext context)
        {
            _entities = context.Users;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _entities.AsNoTracking()
                .ToListAsync();
        }
    }
}