using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <inheritdoc />
public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase, new()
{
    protected readonly DbSet<T> Set;

    /// <summary>
    /// Constructor for initializing a <see cref="GenericRepository{T}"/> class instance
    /// </summary>
    /// <param name="context">Context of the database</param>
    public GenericRepository(DbContext context)
    {
        Set = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Set.AsNoTracking()
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Set.FindAsync(id);
    }

    public void Add(T entity)
    {
        Set.Add(entity);
    }

    public void Update(T entity)
    {
        Set.Update(entity);
    }

    public void Delete(T entity)
    {
        Set.Remove(entity);
    }

    public void DeleteById(Guid id)
    {
        var entity = new T {Id = id};
        Set.Remove(entity);
    }
}