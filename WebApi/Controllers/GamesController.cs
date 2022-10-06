using Business.DataTransferObjects;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Games controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    /// <summary>
    /// Constructor for initializing a <see cref="GamesController"/> class instance
    /// </summary>
    /// <param name="gameService">Game service</param>
    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Create new game
    /// </summary>
    /// <param name="gameCreationDto">Game creation data</param>
    /// <returns>Newly created game</returns>
    /// <response code="201">Returns the newly created game</response>
    /// <response code="400">Game with specified key already exists</response>
    /// <response code="404">
    /// Game image specified by ImageFileName does not exist / 
    /// Genres or platform types specified by ids do not exist
    /// </response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameWithGenresDto>> Create(GameCreationDto gameCreationDto)
    {
        var result = await _gameService.CreateAsync(gameCreationDto);
        return CreatedAtAction(nameof(GetByKey), new {gameKey = result.Key}, result);
    }

    /// <summary>
    /// Update the game
    /// </summary>
    /// <param name="gameId">Guid of the game to be updated</param>
    /// <param name="gameUpdateDto">Game update data</param>
    /// <response code="204">Game has been updated</response>
    /// <response code="400">Game with specified key already exists</response>
    /// <response code="404">
    /// Game specified by <paramref name="gameId"/> not found / 
    /// Game image specified by ImageFileName does not exist / 
    /// Genres or platform types specified by ids do not exist
    /// </response>
    [HttpPut("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid gameId, GameUpdateDto gameUpdateDto)
    {
        await _gameService.UpdateAsync(gameId, gameUpdateDto);
        return NoContent();
    }

    /// <summary>
    /// Get a specific game with details by key
    /// </summary>
    /// <param name="gameKey">Key of the game to be retrieved</param>
    /// <returns>The game specified by <paramref name="gameKey"/></returns>
    /// <response code="200">Returns the game specified by <paramref name="gameKey"/></response>
    /// <response code="404">Game with specified <paramref name="gameKey"/> not found</response>
    [HttpGet("{gameKey}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameWithDetailsDto>> GetByKey(string gameKey)
    {
        return await _gameService.GetByKeyWithDetailsAsync(gameKey);
    }

    /// <summary>
    /// Get all games
    /// </summary>
    /// <returns>Array of games</returns>
    /// <response code="200">Returns the array of games</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GameWithGenresDto>>> GetAll()
    {
        var result = await _gameService.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Delete the game
    /// </summary>
    /// <param name="gameId">Guid of the game to be deleted</param>
    /// <response code="204">Game has been deleted</response>
    /// <response code="404">Game specified by <paramref name="gameId"/> not found</response>
    [HttpDelete("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid gameId)
    {
        await _gameService.DeleteAsync(gameId);
        return NoContent();
    }

    /// <summary>
    /// Download the game
    /// </summary>
    /// <param name="gameKey">Key of the game to be downloaded</param>
    /// <response code="200">Game file</response>
    /// <response code="404">Game specified by <paramref name="gameKey"/> not found</response>
    [HttpGet("{gameKey}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Download(string gameKey)
    {
        var fileStream = await _gameService.DownloadAsync(gameKey);
        return File(fileStream, "application/octet-stream", "stub.exe");
    }

    /// <summary>
    /// Upload game image
    /// </summary>
    /// <param name="file">Image file in png or jpg format</param>
    /// <returns>Result of uploading operation, including image file name</returns>
    /// <response code="200">Returns result of uploading operation, including image id</response>
    /// <response code="400">The image file extension is not supported</response>
    [HttpPost("images")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImageUploadResultDto>> UploadImage(IFormFile file)
    {
        await using var fileStream = file.OpenReadStream();
        return await _gameService.UploadImage(fileStream, file.FileName);
    }

    /// <summary>
    /// Get game image
    /// </summary>
    /// <param name="gameKey">Key of the game which image need to be retrieved</param>
    /// <returns>Image of the game specified by <paramref name="gameKey"/></returns>
    /// <response code="200">Returns the image of the game specified by <paramref name="gameKey"/></response>
    /// <response code="404">
    /// The game specified by <paramref name="gameKey"/> not found / 
    /// Image of the game specified by <paramref name="gameKey"/> not found
    /// </response>
    [HttpGet("{gameKey}/image")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ImageUploadResultDto>> GetImage(string gameKey)
    {
        var (fileStream, fileName) = await _gameService.GetImage(gameKey);
        return File(fileStream, "application/octet-stream", fileName);
    }
}