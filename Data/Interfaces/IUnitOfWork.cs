namespace Data.Interfaces;

/// <summary>
/// Unit of work
/// </summary>
public interface IUnitOfWork
{
    public IGameRepository GameRepository { get; }
    public ICommentRepository CommentRepository { get; }
    public IGenreRepository GenreRepository { get; }
    public IPlatformTypeRepository PlatformTypeRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public IUserRepository UserRepository { get; }

    /// <summary>
    /// Save all changes made through the repositories in the context to the database
    /// </summary>
    public Task SaveAsync();
}