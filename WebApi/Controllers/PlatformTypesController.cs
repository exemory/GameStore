using Business.DataTransferObjects;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformTypesController : ControllerBase
{
    private readonly IPlatformTypeService _platformTypeService;

    public PlatformTypesController(IPlatformTypeService platformTypeService)
    {
        _platformTypeService = platformTypeService;
    }

    /// <summary>
    /// Get all platform types
    /// </summary>
    /// <returns>Array of platform types</returns>
    /// <response code="200">Returns the array of platform types</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll()
    {
        var result = await _platformTypeService.GetAllAsync();
        return Ok(result);
    }
}