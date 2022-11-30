using Business.DataTransferObjects;
using Business.Exceptions;

namespace Business.Interfaces;

/// <summary>
/// Service for users
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Get all registered users
    /// </summary>
    /// <returns>List of all registered users mapped into <see cref="UserInfoDto"/></returns>
    public Task<IEnumerable<UserInfoDto>> GetAllAsync();

    /// <summary>
    /// Update user roles
    /// </summary>
    /// <param name="userId">Guid of the user whose roles need to be updated</param>
    /// <param name="userRolesUpdateDto">New user roles</param>
    /// <exception cref="NotFoundException">
    /// Thrown when the user specified by <paramref name="userId"/> does not exist
    /// </exception>
    public Task UpdateRolesAsync(Guid userId, UserRolesUpdateDto userRolesUpdateDto);
}