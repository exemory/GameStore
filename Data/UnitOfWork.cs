using Data.Interfaces;

namespace Data;

/// <inheritdoc />
public class UnitOfWork : IUnitOfWork
{
    private readonly GameStoreContext _context;

    /// <summary>
    /// Constructor for initializing a <see cref="UnitOfWork"/> class instance
    /// </summary>
    /// <param name="context">Context of the database</param>
    /// <param name="gameRepository">Game repository</param>
    /// <param name="commentRepository">Comment repository</param>
    public UnitOfWork(GameStoreContext context, IGameRepository gameRepository, ICommentRepository commentRepository)
    {
        _context = context;
        GameRepository = gameRepository;
        CommentRepository = commentRepository;
    }

    public IGameRepository GameRepository { get; }
    public ICommentRepository CommentRepository { get; }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}