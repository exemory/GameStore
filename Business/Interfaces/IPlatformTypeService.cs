using Business.DataTransferObjects;

namespace Business.Interfaces;

/// <summary>
/// Service for platform types
/// </summary>
public interface IPlatformTypeService
{
    /// <summary>
    /// Get all platform types
    /// </summary>
    /// <returns>The list of platform types mapped into <see cref="PlatformTypeDto"/></returns>
    public Task<IEnumerable<PlatformTypeDto>> GetAllAsync();
}