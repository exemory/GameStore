using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

/// <inheritdoc />
public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor for initializing a <see cref="CommentService"/> class instance
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="mapper">Mapper</param>
    public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CommentDto> CreateAsync(CommentCreationDto commentCreationDto)
    {
        await CheckIfGameExists(commentCreationDto.GameId);

        if (commentCreationDto.ParentId != null)
        {
            await CheckParentComment(commentCreationDto.ParentId.Value);
        }

        var newComment = _mapper.Map<CommentCreationDto, Comment>(commentCreationDto);

        _unitOfWork.CommentRepository.Add(newComment);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<CommentDto>(newComment);
    }

    public async Task<IEnumerable<CommentDto>> GetAllByGameKeyAsync(string gameKey)
    {
        await CheckIfGameExists(gameKey);

        var comments = await _unitOfWork.CommentRepository.GetAllByGameKeyAsync(gameKey);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    private async Task CheckIfGameExists(Guid gameId)
    {
        var game = await _unitOfWork.GameRepository.GetByIdAsync(gameId);

        if (game == null)
        {
            throw new NotFoundException($"Game with id '{gameId}' not found.");
        }
    }

    private async Task CheckIfGameExists(string gameKey)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyAsync(gameKey);

        if (game == null)
        {
            throw new NotFoundException($"Game with key '{gameKey}' not found.");
        }
    }

    private async Task CheckParentComment(Guid commentId)
    {
        var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentId);

        if (comment == null)
        {
            throw new NotFoundException($"Comment with id '{commentId}' not found.");
        }

        if (comment.GameId != commentId)
        {
            throw new GameStoreException("Parent comment must be from the same game.");
        }
    }
}