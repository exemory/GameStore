using Business.DataTransferObjects;

namespace Business.Interfaces;

/// <summary>
/// Service for genres
/// </summary>
public interface IGenreService
{
    /// <summary>
    /// Get all genres
    /// </summary>
    /// <returns>The list of genres mapped into <see cref="GenreDto"/></returns>
    public Task<IEnumerable<GenreDto>> GetAllAsync();
}