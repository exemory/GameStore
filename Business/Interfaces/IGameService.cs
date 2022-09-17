using Business.DataTransferObjects;
using Business.Exceptions;

namespace Business.Interfaces;

/// <summary>
/// Service for games
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Create new game
    /// </summary>
    /// <param name="gameCreationDto">Game creation data</param>
    /// <returns>Created game mapped into <see cref="GameDto"/></returns>
    public Task<GameDto> CreateAsync(GameCreationDto gameCreationDto);
    
    /// <summary>
    /// Updates the game
    /// </summary>
    /// <param name="id">Guid of the game to be updated</param>
    /// <param name="gameUpdateDto">Game update data</param>
    /// <exception cref="NotFoundException">
    /// Thrown when the game specified by <paramref name="id"/> does not exist
    /// </exception>
    public Task UpdateAsync(Guid id, GameUpdateDto gameUpdateDto);
    
    /// <summary>
    /// Get a specific game with details by it's key
    /// </summary>
    /// <param name="key">Key of the game to be retrieved</param>
    /// <returns>The game mapped into <see cref="GameWithDetailsDto"/></returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the game with specified <paramref name="key"/> does not exist
    /// </exception>
    public Task<GameWithDetailsDto> GetByKeyWithDetailsAsync(string key);

    /// <summary>
    /// Get all games
    /// </summary>
    /// <returns>The list of games mapped into <see cref="GameDto"/></returns>
    public Task<IEnumerable<GameDto>> GetAllAsync();
    
    /// <summary>
    /// Delete the game
    /// </summary>
    /// <param name="id">Guid of the game to be deleted</param>
    /// <exception cref="NotFoundException">
    /// Thrown when the game with specified <paramref name="id"/> does not exist
    /// </exception>
    public Task DeleteAsync(Guid id);

    /// <summary>
    /// Get all games by specified genre
    /// </summary>
    /// <param name="genre">Game genre</param>
    /// <returns>Games mapped into <see cref="GameDto"/></returns>
    public Task<IEnumerable<GameDto>> GetAllByGenreAsync(string genre);
    
    /// <summary>
    /// Get all games by specified platform type
    /// </summary>
    /// <param name="platformTypes">List of platform types</param>
    /// <returns>Games mapped into <see cref="GameDto"/></returns>
    public Task<IEnumerable<GameDto>> GetAllByPlatformTypesAsync(IEnumerable<string> platformTypes);
}