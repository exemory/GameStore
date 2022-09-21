using Data.Entities;

namespace Data.Interfaces;

/// <summary>
/// Generic repository
/// </summary>
/// <typeparam name="T">Entity type, inherited from <see cref="EntityBase"/></typeparam>
public interface IRepository<T> where T : EntityBase
{
    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>List of entities</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Get entity specified by <paramref name="id"/>
    /// </summary>
    /// <param name="id">Id of the entity to be retrieved</param>
    /// <returns>
    /// Entity specified by <paramref name="id"/>, if exists, <c>null</c> otherwise
    /// </returns>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Add entity
    /// </summary>
    /// <param name="entity">Entity to be started tracking by context as added</param>
    void Add(T entity);

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entity">Entity to be started tracking by context as modified</param>
    void Update(T entity);

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity">Entity to be started tracking by context as deleted</param>
    void Delete(T entity);

    /// <summary>
    /// Delete entity specified by <paramref name="id"/>
    /// </summary>
    /// <param name="id">Guid of the entity to be started tracking by context as deleted</param>
    void DeleteById(Guid id);
}