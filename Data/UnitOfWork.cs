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
    /// <param name="genreRepository">Genre repository</param>
    public UnitOfWork(GameStoreContext context, IGameRepository gameRepository, ICommentRepository commentRepository,
        IGenreRepository genreRepository)
    {
        _context = context;
        GameRepository = gameRepository;
        CommentRepository = commentRepository;
        GenreRepository = genreRepository;
    }

    public IGameRepository GameRepository { get; }
    public ICommentRepository CommentRepository { get; }
    public IGenreRepository GenreRepository { get; }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}