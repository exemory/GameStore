using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <inheritdoc />
public class Repository<T> : IRepository<T> where T : EntityBase, new()
{
    protected readonly GameStoreContext Context;
    protected readonly DbSet<T> Entities;

    /// <summary>
    /// Constructor for initializing a <see cref="Repository{T}"/> class instance
    /// </summary>
    /// <param name="context">Context of the database</param>
    public Repository(GameStoreContext context)
    {
        Context = context;
        Entities = Context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Entities.AsNoTracking()
            .ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Entities.FindAsync(id);
    }

    public void Add(T entity)
    {
        Entities.Add(entity);
    }

    public void Update(T entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        Entities.Remove(entity);
    }

    public void DeleteById(Guid id)
    {
        var entity = new T {Id = id};
        Entities.Remove(entity);
    }
}