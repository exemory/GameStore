using Business.DataTransferObjects;

namespace Business.Interfaces;

public interface ICommentService
{
    public Task<CommentDto> CreateCommentAsync(CommentCreationDto commentCreationDto);
    
    public Task<IEnumerable<CommentDto>> GetAllCommentsByGameKeyAsync(string gameKey);
}