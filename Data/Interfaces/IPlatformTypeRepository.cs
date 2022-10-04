using Data.Entities;

namespace Data.Interfaces;

/// <summary>
/// Repository of <see cref="PlatformType"/> entities
/// </summary>
public interface IPlatformTypeRepository : IRepository<PlatformType>
{
    /// <summary>
    /// Get platform types by their ids
    /// </summary>
    /// <param name="ids">Guids of platform types need to be retrieved</param>
    /// <returns>Platform types specified by <paramref name="ids"/></returns>
    public Task<ICollection<PlatformType>> GetByIdsAsync(IEnumerable<Guid> ids);
}