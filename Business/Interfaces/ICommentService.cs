using Business.DataTransferObjects;
using Business.Exceptions;

namespace Business.Interfaces;

/// <summary>
/// Service for game comments
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// Create new game comment
    /// </summary>
    /// <param name="commentCreationDto">Comment creation data</param>
    /// <returns>Created comment mapped into <see cref="CommentDto"/></returns>
    /// <exception cref="NotFoundException">
    /// Thrown when:
    /// <list type="bullet">
    /// <item><description>The game with specified id does not exist</description></item>
    /// <item><description>Parent comment with specified id does not exist</description></item>
    /// </list>
    /// </exception>
    /// <exception cref="GameStoreException">
    /// Thrown when parent comment does not belong to the specified game
    /// </exception>
    public Task<CommentDto> CreateAsync(CommentCreationDto commentCreationDto);
    
    /// <summary>
    /// Get all game's comment by game key
    /// </summary>
    /// <param name="gameKey">Key of the game whose comments need to be retrieved</param>
    /// <returns>Comments mapped into <see cref="GameWithGenresDto"/></returns>
    public Task<IEnumerable<CommentDto>> GetAllByGameKeyAsync(string gameKey);
}