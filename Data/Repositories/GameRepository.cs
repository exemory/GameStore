using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <inheritdoc cref="IGameRepository" />
public class GameRepository : Repository<Game>, IGameRepository
{
    /// <summary>
    /// Constructor for initializing a <see cref="GameRepository"/> class instance
    /// </summary>
    /// <param name="context">Context of the database</param>
    public GameRepository(GameStoreContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Game>> GetAllWithGenresAsync()
    {
        return await Entities.Include(g => g.Genres)
            .ToListAsync();
    }
    
    public async Task<Game?> GetByKeyAsync(string key)
    {
        return await Entities.Where(g => g.Key == key)
            .FirstOrDefaultAsync();
    }
    
    public async Task<Game?> GetByKeyWithDetailsAsync(string key)
    {
        return await Entities.Include(g => g.Genres)
            .Include(g => g.PlatformTypes)
            .Where(g => g.Key == key)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Game>> GetAllByGenreWithGenresAsync(string genre)
    {
        return await Entities.Include(g => g.Genres)
            .Where(g => g.Genres.Any(g2 => g2.Name == genre))
            .ToListAsync();
    }

    public async Task<IEnumerable<Game>> GetAllByPlatformTypesWithGenresAsync(IEnumerable<string> platformTypes)
    {
        return await Entities.Include(g => g.Genres)
            .Where(g => g.PlatformTypes.All(pt => platformTypes.Contains(pt.Type)))
            .ToListAsync();
    }
}