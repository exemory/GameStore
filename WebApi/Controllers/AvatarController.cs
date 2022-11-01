using Business.DataTransferObjects;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AvatarController : ControllerBase
{
    private readonly IAvatarService _avatarService;

    public AvatarController(IAvatarService avatarService)
    {
        _avatarService = avatarService;
    }

    /// <summary>
    /// Upload user avatar image
    /// </summary>
    /// <param name="file">Image file in png or jpg format</param>
    /// <returns>Result of uploading operation, including image file name</returns>
    /// <response code="201">Avatar image has been uploaded</response>
    /// <response code="400">The image file extension is not supported</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImageUploadResultDto>> Upload(IFormFile file)
    {
        await using var fileStream = file.OpenReadStream();
        await _avatarService.UploadAvatarImageAsync(fileStream, file.FileName);
        return CreatedAtAction(nameof(Get), null);
    }

    /// <summary>
    /// Get user's avatar image
    /// </summary>
    /// <param name="username">
    /// Username of the user whose avatar need to be retrieved, if specified,
    /// otherwise will be returned avatar of the current authorized user
    /// </param>
    /// <returns>Avatar image of the authorized user</returns>
    /// <response code="200">Returns the avatar image of the authorized user</response>
    /// <response code="403">The user is not authorized</response>
    /// <response code="404">
    /// The user with specified username not found / 
    /// The user has not avatar / 
    /// The user's avatar not found
    /// </response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImageUploadResultDto>> Get(string? username)
    {
        var (fileStream, fileName) = await _avatarService.GetAvatarImageAsync(username);
        return File(fileStream, "application/octet-stream", fileName);
    }
}