using Business.DataTransferObjects;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all registered users
    /// </summary>
    /// <returns>Array of registered users</returns>
    /// <response code="200">Returns the array of registered users</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetAll()
    {
        return Ok(await _userService.GetAllAsync());
    }

    /// <summary>
    /// Update user roles
    /// </summary>
    /// <param name="userId">Guid of the user whose roles need to be updated</param>
    /// <param name="userRolesUpdateDto">New user roles</param>
    /// <response code="204">Roles have been updated</response>
    /// <response code="404">The user specified by <paramref name="userId"/> not found</response>
    [HttpPut("{userId:guid}/roles")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoles(Guid userId, UserRolesUpdateDto userRolesUpdateDto)
    {
        await _userService.UpdateRolesAsync(userId, userRolesUpdateDto);
        return NoContent();
    }
}