﻿using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <inheritdoc cref="ICommentRepository" />
public class CommentRepository : Repository<Comment>, ICommentRepository
{
    /// <summary>
    /// Constructor for initializing a <see cref="CommentRepository"/> class instance
    /// </summary>
    /// <param name="context">Context of the database</param>
    public CommentRepository(GameStoreContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Comment>> GetAllByGameKeyAsync(string gameKey)
    {
        return await Entities.Include(c => c.User)
            .Where(c => c.Game.Key == gameKey && !c.Deleted)
            .OrderByDescending(c => c.CreationDate)
            .ToListAsync();
    }
}