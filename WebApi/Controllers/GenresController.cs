using Business.DataTransferObjects;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    /// <summary>
    /// Get all genres
    /// </summary>
    /// <returns>Array of genres</returns>
    /// <response code="200">Returns the array of genres</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll()
    {
        var result = await _genreService.GetAllAsync();
        return Ok(result);
    }
}