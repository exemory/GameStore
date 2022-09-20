using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <inheritdoc cref="IGameRepository" />
public class GameRepository : GenericRepository<Game>, IGameRepository
{
    /// <summary>
    /// Constructor for initializing a <see cref="GameRepository"/> class instance
    /// </summary>
    /// <param name="context">Context of the database</param>
    public GameRepository(GameStoreContext context) : base(context)
    {
    }

    public async Task<Game?> GetByKeyAsync(string key)
    {
        return await Set.Where(g => g.Key == key)
            .FirstOrDefaultAsync();
    }
    
    public async Task<Game?> GetByKeyWithDetailsAsync(string key)
    {
        return await Set.Include(g => g.Genres)
            .Include(g => g.PlatformTypes)
            .Where(g => g.Key == key)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Game>> GetAllByGenreAsync(string genre)
    {
        return await Set.Where(g => g.Genres.Any(g2 => g2.Name == genre))
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetAllByPlatformTypesAsync(IEnumerable<string> platformTypes)
    {
        return await Set.Where(g => g.PlatformTypes.All(pt => platformTypes.Contains(pt.Type)))
            .ToListAsync();
    }
}