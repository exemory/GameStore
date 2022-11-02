using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Business.Services;

/// <inheritdoc />
public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISession _session;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<CommentService> _logger;

    /// <summary>
    /// Constructor for initializing a <see cref="CommentService"/> class instance
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="mapper">Mapper</param>
    /// <param name="session">Current user's session</param>
    /// <param name="userManager">Identity user manager</param>
    /// <param name="logger">Logger</param>
    public CommentService(IUnitOfWork unitOfWork, IMapper mapper, ISession session, UserManager<User> userManager,
        ILogger<CommentService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _session = session;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<CommentDto> CreateAsync(CommentCreationDto commentCreationDto)
    {
        await CheckBeforeCommentCreationAsync(commentCreationDto.GameId, commentCreationDto.ParentId);

        var user = await GetAuthorizedUserAsync();

        var newComment = _mapper.Map<CommentCreationDto, Comment>(commentCreationDto);
        newComment.User = user;

        _unitOfWork.CommentRepository.Add(newComment);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<CommentDto>(newComment);
    }

    private async Task CheckBeforeCommentCreationAsync(Guid gameId, Guid? parentCommentId)
    {
        await CheckIfGameExistsAsync(gameId);

        if (parentCommentId != null)
        {
            await CheckParentCommentAsync(parentCommentId.Value, gameId);
        }
    }

    private async Task<User> GetAuthorizedUserAsync()
    {
        var user = await _userManager.FindByIdAsync(_session.UserId.ToString());

        if (user == null)
        {
            _logger.LogError("Authorized user with id '{UserId}' does not exist", _session.UserId);
            throw new AccessDeniedException("User is not authorized.");
        }

        return user;
    }

    public async Task<IEnumerable<CommentDto>> GetAllByGameKeyAsync(string gameKey)
    {
        await CheckIfGameExistsAsync(gameKey);

        var comments = await _unitOfWork.CommentRepository.GetAllByGameKeyAsync(gameKey);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task EditAsync(Guid commentId, CommentUpdateDto commentUpdateDto)
    {
        var comment = await GetCommentForEditingAsync(commentId);

        _mapper.Map(commentUpdateDto, comment);

        _unitOfWork.CommentRepository.Update(comment);
        await _unitOfWork.SaveAsync();
    }

    private async Task<Comment> GetCommentForEditingAsync(Guid commentId)
    {
        var comment = await GetCommentByIdAsync(commentId);

        if (comment.Deleted)
        {
            ThrowCommentNotFound(commentId);
        }

        if (comment.UserId != _session.UserId)
        {
            throw new AccessDeniedException("You can only edit your own comments.");
        }

        return comment;
    }

    public async Task DeleteAsync(Guid commentId)
    {
        var comment = await GetCommentForDeletingAsync(commentId);

        comment.Deleted = true;

        _unitOfWork.CommentRepository.Update(comment);
        await _unitOfWork.SaveAsync();
    }

    private async Task<Comment> GetCommentForDeletingAsync(Guid commentId)
    {
        var comment = await GetCommentByIdAsync(commentId);

        if (comment.UserId != _session.UserId)
        {
            throw new AccessDeniedException("You can only delete your own comments.");
        }

        if (comment.Deleted)
        {
            throw new GameStoreException($"The comment with id '{commentId}' has already been deleted.");
        }

        return comment;
    }

    public async Task RestoreAsync(Guid commentId)
    {
        var comment = await GetCommentForRestoringAsync(commentId);

        comment.Deleted = false;

        _unitOfWork.CommentRepository.Update(comment);
        await _unitOfWork.SaveAsync();
    }

    private async Task<Comment> GetCommentForRestoringAsync(Guid commentId)
    {
        var comment = await GetCommentByIdAsync(commentId);

        if (comment.UserId != _session.UserId)
        {
            throw new AccessDeniedException("You can only restore your own comments.");
        }

        if (!comment.Deleted)
        {
            throw new GameStoreException($"The comment with id '{commentId}' is not deleted.");
        }

        return comment;
    }

    private async Task CheckIfGameExistsAsync(Guid gameId)
    {
        var game = await _unitOfWork.GameRepository.GetByIdAsync(gameId);

        if (game == null)
        {
            throw new NotFoundException($"Game with id '{gameId}' not found.");
        }
    }

    private async Task CheckIfGameExistsAsync(string gameKey)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyAsync(gameKey);

        if (game == null)
        {
            throw new NotFoundException($"Game with key '{gameKey}' not found.");
        }
    }

    private async Task CheckParentCommentAsync(Guid commentId, Guid gameId)
    {
        var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentId);

        if (comment == null)
        {
            ThrowCommentNotFound(commentId);
        }

        if (comment!.GameId != gameId)
        {
            throw new GameStoreException("Parent comment must be from the same game.");
        }
    }

    private async Task<Comment> GetCommentByIdAsync(Guid commentId)
    {
        var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentId);

        if (comment == null)
        {
            ThrowCommentNotFound(commentId);
        }

        return comment!;
    }

    private static void ThrowCommentNotFound(Guid commentId)
    {
        throw new NotFoundException($"Comment with id '{commentId}' not found.");
    }
}