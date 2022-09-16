using Data.Entities;

namespace Data.Interfaces;

/// <summary>
/// Repository of <see cref="Game"/> entities
/// </summary>
public interface IGameRepository : IRepository<Game>
{
    /// <summary>
    /// Get a specific game with details by it's key
    /// </summary>
    /// <param name="key">Key of the game to be retrieved</param>
    /// <returns><see cref="Game"/> including it's genres and platform types</returns>
    public Task<Game?> GetByKeyWithDetailsAsync(string key);
    
    /// <summary>
    /// Get all games by genre
    /// </summary>
    /// <param name="genre">Game genre</param>
    /// <returns>All games of specified genre</returns>
    public Task<IEnumerable<Game>> GetAllByGenreAsync(string genre);
    
    /// <summary>
    /// Get all games by platform types
    /// </summary>
    /// <param name="platformTypes">List of platform types</param>
    /// <returns>All games supported on specified platform types</returns>
    public Task<IEnumerable<Game>> GetAllByPlatformTypesAsync(IEnumerable<string> platformTypes);
}