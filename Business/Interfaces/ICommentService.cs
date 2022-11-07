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
    /// <exception cref="AccessDeniedException">
    /// Thrown when the authorized user not found
    /// </exception>
    public Task<CommentDto> CreateAsync(CommentCreationDto commentCreationDto);

    /// <summary>
    /// Get all game's comment by game key
    /// </summary>
    /// <param name="gameKey">Key of the game whose comments need to be retrieved</param>
    /// <returns>Comments mapped into <see cref="GameWithGenresDto"/></returns>
    public Task<IEnumerable<CommentDto>> GetAllByGameKeyAsync(string gameKey);

    /// <summary>
    /// Edit the comment
    /// </summary>
    /// <param name="commentId">Guid of the comment which need to be updated</param>
    /// <param name="commentUpdateDto">Comment update data</param>
    /// <exception cref="NotFoundException">
    /// Thrown when comment specified by <paramref name="commentId"/> does not exist
    /// </exception>
    /// <exception cref="AccessDeniedException">
    /// Thrown when user tries to edit other user's comment
    /// </exception>
    public Task EditAsync(Guid commentId, CommentUpdateDto commentUpdateDto);

    /// <summary>
    /// Mark the comment specified by <paramref name="commentId"/> as deleted
    /// </summary>
    /// <param name="commentId">Guid of the comment which need to be deleted</param>
    /// <exception cref="NotFoundException">
    /// Thrown when comment specified by <paramref name="commentId"/> does not exist
    /// </exception>
    /// <exception cref="AccessDeniedException">
    /// Thrown when user tries to delete other user's comment
    /// </exception>
    /// <exception cref="GameStoreException">
    /// Thrown when the comment has already been deleted
    /// </exception>
    public Task DeleteAsync(Guid commentId);

    /// <summary>
    /// Restore the comment that has been marked as deleted
    /// </summary>
    /// <param name="commentId">Guid of the comment which need to be restored</param>
    /// <exception cref="NotFoundException">
    /// Thrown when comment specified by <paramref name="commentId"/> does not exist
    /// </exception>
    /// <exception cref="AccessDeniedException">
    /// Thrown when user tries to restore other user's comment
    /// </exception>
    /// <exception cref="GameStoreException">
    /// Thrown when the comment is not deleted
    /// </exception>
    public Task RestoreAsync(Guid commentId);
}