using Data.Entities;

namespace Data.Interfaces;

/// <summary>
/// Repository of <see cref="Comment"/> entities
/// </summary>
public interface ICommentRepository : IRepository<Comment>
{
    /// <summary>
    /// Get all game's comments by specified game key including users
    /// </summary>
    /// <param name="gameKey">Key of the game whose comments need to be retrieved</param>
    /// <returns>Comments of specified game including users</returns>
    public Task<IEnumerable<Comment>> GetAllByGameKeyAsync(string gameKey);
}