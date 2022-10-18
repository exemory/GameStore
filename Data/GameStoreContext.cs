﻿using Data.Configurations;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class GameStoreContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public GameStoreContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Game> Games { get; set; } = default!;
    public DbSet<Comment> Comments { get; set; } = default!;
    public DbSet<Genre> Genres { get; set; } = default!;
    public DbSet<PlatformType> PlatformTypes { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new GameConfiguration());
        builder.ApplyConfiguration(new CommentConfiguration());
        builder.ApplyConfiguration(new GenreConfiguration());
        builder.ApplyConfiguration(new PlatformTypeConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
    }
}