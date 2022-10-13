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
    public async Task<ActionResult<ImageUploadResultDto>> UploadImageAvatar(IFormFile file)
    {
        await using var fileStream = file.OpenReadStream();
        await _avatarService.UploadAvatarImage(fileStream, file.FileName);
        return CreatedAtAction(nameof(GetImage), null);
    }

    /// <summary>
    /// Get authorized user's avatar image
    /// </summary>
    /// <returns>Avatar image of the authorized user</returns>
    /// <response code="200">Returns the avatar image of the authorized user</response>
    /// <response code="404">
    /// The user has not avatar / 
    /// The user's avatar not found
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImageUploadResultDto>> GetImage()
    {
        var (fileStream, fileName) = await _avatarService.GetAvatarImage();
        return File(fileStream, "application/octet-stream", fileName);
    }
}