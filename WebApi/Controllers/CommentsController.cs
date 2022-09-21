using System.ComponentModel.DataAnnotations;
using Business.DataTransferObjects;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// Comments controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    /// <summary>
    /// Constructor for initializing a <see cref="GamesController"/> class instance
    /// </summary>
    /// <param name="commentService">Comment service</param>
    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    /// <summary>
    /// Create game comment
    /// </summary>
    /// <param name="commentCreationDto">Comment creation data</param>
    /// <returns>Newly created comment</returns>
    /// <response code="201">Returns the newly created comment</response>
    /// <response code="400">Parent comment must be from the same game</response>
    /// <response code="404">Specified game or parent comment not found</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> NewComment(CommentCreationDto commentCreationDto)
    {
        var result = await _commentService.CreateAsync(commentCreationDto);
        return CreatedAtAction(null, result);
    }
    
    /// <summary>
    /// Get all game's comments
    /// </summary>
    /// <param name="gameKey">Key of the game whose comments need to be retrieved</param>
    /// <returns>Array of game's comments</returns>
    /// <response code="200">Returns the array of games</response>
    /// <response code="404">The game specified by <paramref name="gameKey"/> not found</response>
    [HttpGet]
    [ResponseCache(Duration = 60)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> GetAllByGameKey([Required] string gameKey)
    {
        var result = await _commentService.GetAllByGameKeyAsync(gameKey);
        return Ok(result);
    }
}