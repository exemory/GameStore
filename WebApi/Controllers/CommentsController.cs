using System.ComponentModel.DataAnnotations;
using Business.DataTransferObjects;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> New(CommentCreationDto commentCreationDto)
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> GetAllByGameKey([Required] string gameKey)
    {
        var result = await _commentService.GetAllByGameKeyAsync(gameKey);
        return Ok(result);
    }

    /// <summary>
    /// Edit the comment
    /// </summary>
    /// <param name="commentId">Guid of the comment which need to be edited</param>
    /// <param name="commentUpdateDto">Comment update data</param>
    /// <response code="204">Comment has been updated</response>
    /// <response code="403">User tries to edit other user's comment</response>
    /// <response code="404">The comment specified by <paramref name="commentId"/> not found</response>
    [HttpPut("{commentId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> Edit(Guid commentId, CommentUpdateDto commentUpdateDto)
    {
        await _commentService.EditAsync(commentId, commentUpdateDto);
        return NoContent();
    }

    /// <summary>
    /// Delete the comment
    /// </summary>
    /// <param name="commentId">Guid of the comment which need to be deleted</param>
    /// <response code="204">Comment has been deleted</response>
    /// <response code="400">The comment has already been deleted</response>
    /// <response code="403">User tries to delete other user's comment</response>
    /// <response code="404">The comment specified by <paramref name="commentId"/> not found</response>
    [HttpDelete("{commentId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> Delete(Guid commentId)
    {
        await _commentService.DeleteAsync(commentId);
        return NoContent();
    }

    /// <summary>
    /// Restore the comment
    /// </summary>
    /// <param name="commentId">Guid of the comment which need to be restored</param>
    /// <response code="204">The comment has been restored</response>
    /// <response code="400">The comment is not deleted</response>
    /// <response code="403">User tries to restore other user's comment</response>
    /// <response code="404">The comment specified by <paramref name="commentId"/> not found</response>
    [HttpPut("{commentId:guid}/restore")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> Restore(Guid commentId)
    {
        await _commentService.RestoreAsync(commentId);
        return NoContent();
    }
}