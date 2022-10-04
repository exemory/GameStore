using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <inheritdoc cref="IPlatformTypeRepository" />
public class PlatformTypeRepository : Repository<PlatformType>, IPlatformTypeRepository
{
    /// <summary>
    /// Constructor for initializing a <see cref="PlatformTypeRepository"/> class instance
    /// </summary>
    /// <param name="context">Context of the database</param>
    public PlatformTypeRepository(GameStoreContext context) : base(context)
    {
    }

    public async Task<ICollection<PlatformType>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await Entities.Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }
}