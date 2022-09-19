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
        var game = await _unitOfWork.GameRepository.GetByIdAsync(commentCreationDto.GameId);

        if (game == null)
        {
            throw new NotFoundException($"Game with id '{commentCreationDto.GameId}' not found.");
        }

        if (commentCreationDto.ParentId != null)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentCreationDto.ParentId.Value);

            if (comment == null)
            {
                throw new NotFoundException($"Comment with id '{commentCreationDto.ParentId.Value}' not found.");
            }

            if (comment.GameId != commentCreationDto.GameId)
            {
                throw new GameStoreException($"Parent comment must be from the same game.");
            }
        }

        var newComment = _mapper.Map<CommentCreationDto, Comment>(commentCreationDto);
        
        _unitOfWork.CommentRepository.Add(newComment);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<CommentDto>(newComment);
    }

    public async Task<IEnumerable<CommentDto>> GetAllByGameKeyAsync(string gameKey)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyWithDetailsAsync(gameKey);

        if (game == null)
        {
            throw new NotFoundException($"Game with key '{gameKey}' not found.");
        }

        var comments = await _unitOfWork.CommentRepository.GetAllByGameKeyAsync(gameKey);
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }
}