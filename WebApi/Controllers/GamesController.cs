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
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<GameDto>> Create(GameCreationDto gameCreationDto)
    {
        var result = await _gameService.CreateAsync(gameCreationDto);
        return CreatedAtAction(nameof(GetByKey), new {key = result.Key}, result);
    }
    
    /// <summary>
    /// Update the game
    /// </summary>
    /// <param name="id">Guid of the game to be updated</param>
    /// <param name="gameUpdateDto">Game update data</param>
    /// <response code="204">Game has been updated</response>
    /// <response code="404">Game specified by <paramref name="id"/> not found</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid id, GameUpdateDto gameUpdateDto)
    {
        await _gameService.UpdateAsync(id, gameUpdateDto);
        return NoContent();
    }
    
    /// <summary>
    /// Get a specific game by key
    /// </summary>
    /// <param name="key">Key of the game to be retrieved</param>
    /// <returns>The game specified by <paramref name="key"/></returns>
    /// <response code="200">Returns the game specified by <paramref name="key"/></response>
    /// <response code="404">Game with specified <paramref name="key"/> not found</response>
    [HttpGet("{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameWithDetailsDto>> GetByKey(string key)
    {
        return await _gameService.GetByKeyWithDetailsAsync(key);
    }
    
    /// <summary>
    /// Get all games
    /// </summary>
    /// <returns>Array of games</returns>
    /// <response code="200">Returns the array of games</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetAll()
    {
        var result = await _gameService.GetAllAsync();
        return Ok(result);
    }
    
    /// <summary>
    /// Delete the thread
    /// </summary>
    /// <param name="id">Guid of the game to be deleted</param>
    /// <response code="204">Game has been deleted</response>
    /// <response code="404">Game specified by <paramref name="id"/> not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _gameService.DeleteAsync(id);
        return NoContent();
    }
    
    /// <summary>
    /// Download the game
    /// </summary>
    /// <param name="key">Key of the game to be downloaded</param>
    /// <response code="200">Game file</response>
    [HttpGet("{key}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Download(string key)
    {
        var fileStream = await _gameService.Download(key);
        return File(fileStream, "application/octet-stream", "stub.exe");
    }
}