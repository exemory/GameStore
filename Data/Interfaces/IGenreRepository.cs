using Data.Entities;

namespace Data.Interfaces;

/// <summary>
/// Repository of <see cref="Genre"/> entities
/// </summary>
public interface IGenreRepository : IRepository<Genre>
{
    /// <summary>
    /// Get genres by their ids
    /// </summary>
    /// <param name="ids">Guids of genres need to be retrieved</param>
    /// <returns>Genres specified by <paramref name="ids"/></returns>
    public Task<ICollection<Genre>> GetByIdsAsync(IEnumerable<Guid> ids);
}