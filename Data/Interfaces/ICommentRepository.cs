using Data.Entities;

namespace Data.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    /// <summary>
    /// Get all game's comments by specified game key
    /// </summary>
    /// <param name="gameKey">Game key which comment are to be retrieved</param>
    /// <returns>Comments of specified game</returns>
    public Task<IEnumerable<Comment>> GetAllByGameKeyAsync(string gameKey);
}