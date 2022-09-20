using Business.Interfaces;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class DbInitializer : IDbInitializer
{
    private readonly GameStoreContext _context;

    public DbInitializer(GameStoreContext context)
    {
        _context = context;
    }

    public async Task Initialize()
    {
        await _context.Database.MigrateAsync();
    }
}