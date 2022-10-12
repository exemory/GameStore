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
    /// <returns>Created game mapped into <see cref="GameWithGenresDto"/></returns>
    /// <exception cref="GameStoreException">
    /// Thrown when the game with specified key already exists
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when:
    /// <list type="bullet">
    /// <item><description>Game image specified by ImageFileName does not exist</description></item>
    /// <item><description>Genres with specified ids do not exist</description></item>
    /// <item><description>Platform types with specified ids do not exist</description></item>
    /// </list>
    /// </exception>
    public Task<GameWithGenresDto> CreateAsync(GameCreationDto gameCreationDto);

    /// <summary>
    /// Update the game
    /// </summary>
    /// <param name="gameId">Guid of the game to be updated</param>
    /// <param name="gameUpdateDto">Game update data</param>
    /// <exception cref="GameStoreException">
    /// Thrown when the game with specified key already exists
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when:
    /// <list type="bullet">
    /// <item><description>The game specified by <paramref name="gameId"/> does not exist</description></item>
    /// <item><description>Game image specified by ImageFileName does not exist</description></item>
    /// <item><description>Genres with specified ids do not exist</description></item>
    /// <item><description>Platform types with specified ids do not exist</description></item>
    /// </list>
    /// </exception>
    public Task UpdateAsync(Guid gameId, GameUpdateDto gameUpdateDto);

    /// <summary>
    /// Get a specific game with details by it's key
    /// </summary>
    /// <param name="gameKey">Key of the game to be retrieved</param>
    /// <returns>The game mapped into <see cref="GameWithDetailsDto"/></returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the game with specified <paramref name="gameKey"/> does not exist
    /// </exception>
    public Task<GameWithDetailsDto> GetByKeyWithDetailsAsync(string gameKey);

    /// <summary>
    /// Get all games
    /// </summary>
    /// <returns>The list of games mapped into <see cref="GameWithGenresDto"/></returns>
    public Task<IEnumerable<GameWithGenresDto>> GetAllAsync();

    /// <summary>
    /// Delete the game
    /// </summary>
    /// <param name="gameId">Guid of the game to be deleted</param>
    /// <exception cref="NotFoundException">
    /// Thrown when the game with specified <paramref name="gameId"/> does not exist
    /// </exception>
    public Task DeleteAsync(Guid gameId);

    /// <summary>
    /// Get all games by specified genre
    /// </summary>
    /// <param name="genre">Game genre</param>
    /// <returns>Games mapped into <see cref="GameWithGenresDto"/></returns>
    public Task<IEnumerable<GameWithGenresDto>> GetAllByGenreAsync(string genre);

    /// <summary>
    /// Get all games by specified platform type
    /// </summary>
    /// <param name="platformTypes">List of platform types</param>
    /// <returns>Games mapped into <see cref="GameWithGenresDto"/></returns>
    public Task<IEnumerable<GameWithGenresDto>> GetAllByPlatformTypesAsync(IEnumerable<string> platformTypes);

    /// <summary>
    /// Download game by game key
    /// </summary>
    /// <param name="gameKey">Key of the game to be downloaded</param>
    /// <returns>File stream of the game</returns>
    public Task<Stream> DownloadAsync(string gameKey);

    /// <summary>
    /// Store the uploaded game image
    /// </summary>
    /// <param name="fileStream">Stream of the image file</param>
    /// <param name="originalFileName">User-specified name of the uploading image</param>
    /// <returns>Result of image uploading including it's file name</returns>
    /// <exception cref="GameStoreException">
    /// Thrown when the image file extension is not supported
    /// </exception>
    public Task<ImageUploadResultDto> UploadImage(Stream fileStream, string originalFileName);

    /// <summary>
    /// Get game image
    /// </summary>
    /// <param name="gameKey">Key of the game whose image need to be retrieved</param>
    /// <returns>Tuple that contains the image file stream and file name</returns>
    /// <exception cref="NotFoundException">
    /// Thrown when:
    /// <list type="bullet">
    /// <item><description>The game specified by <paramref name="gameKey"/> does not exist</description></item>
    /// <item><description>Image of the game specified by <paramref name="gameKey"/> not found</description></item>
    /// </list>
    /// </exception>
    public Task<(Stream FileStream, string FileName)> GetImage(string gameKey);
}