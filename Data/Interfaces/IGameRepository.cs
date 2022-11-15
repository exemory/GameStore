using Data.Entities;

namespace Data.Interfaces;

/// <summary>
/// Repository of <see cref="Game"/> entities
/// </summary>
public interface IGameRepository : IRepository<Game>
{
    /// <summary>
    /// Get all games with included genres
    /// </summary>
    /// <returns>All games with included genres</returns>
    public Task<IEnumerable<Game>> GetAllWithGenresAsync();
    
    /// <summary>
    /// Get a specific game by it's key
    /// </summary>
    /// <param name="key">Key of the game to be retrieved</param>
    /// <returns><see cref="Game"/></returns>
    public Task<Game?> GetByKeyAsync(string key);

    /// <summary>
    /// Get a specific game with details by it's id
    /// </summary>
    /// <param name="id">Guid of the game to be retrieved</param>
    /// <returns><see cref="Game"/> with included genres and platform types</returns>
    public Task<Game?> GetByIdWithDetailsAsync(Guid id);
    
    /// <summary>
    /// Get a specific game with details by it's key
    /// </summary>
    /// <param name="key">Key of the game to be retrieved</param>
    /// <returns><see cref="Game"/> including it's genres and platform types</returns>
    public Task<Game?> GetByKeyWithDetailsAsync(string key);
    
    /// <summary>
    /// Get all games by genre with included genres
    /// </summary>
    /// <param name="genre">Game genre</param>
    /// <returns>All games of specified genre with included genres</returns>
    public Task<IEnumerable<Game>> GetAllByGenreWithGenresAsync(string genre);
    
    /// <summary>
    /// Get all games by platform types with included genres
    /// </summary>
    /// <param name="platformTypes">List of platform types</param>
    /// <returns>All games supported on specified platform types with included genres</returns>
    public Task<IEnumerable<Game>> GetAllByPlatformTypesWithGenresAsync(IEnumerable<string> platformTypes);
    
    /// <summary>
    /// Get games by their ids
    /// </summary>
    /// <param name="gameIds">List of game identifiers</param>
    /// <returns>Games with ids specified by <paramref name="gameIds"/></returns>
    public Task<ICollection<Game>> GetByIds(IEnumerable<Guid> gameIds);
}