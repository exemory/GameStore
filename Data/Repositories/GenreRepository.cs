using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

/// <inheritdoc cref="IGenreRepository" />
public class GenreRepository : Repository<Genre>, IGenreRepository
{
    /// <summary>
    /// Constructor for initializing a <see cref="GenreRepository"/> class instance
    /// </summary>
    /// <param name="context">Context of the database</param>
    public GenreRepository(GameStoreContext context) : base(context)
    {
    }
}