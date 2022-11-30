using Business.Interfaces;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class DbInitializer : IDbInitializer
{
    private readonly GameStoreContext _context;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public DbInitializer(GameStoreContext context, RoleManager<Role> roleManager,
        UserManager<User> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task Initialize()
    {
        await _context.Database.MigrateAsync();
        await SeedRequiredData();
    }

    private async Task SeedRequiredData()
    {
        await SeedRoles();
        await AddAdmin();
    }

    private async Task SeedRoles()
    {
        if (!_roleManager.Roles.Any())
        {
            foreach (var roleName in RequiredData.Roles)
            {
                await _roleManager.CreateAsync(new Role(roleName));
            }
        }
    }

    private async Task AddAdmin()
    {
        var adminExists = await _userManager.FindByNameAsync(RequiredData.Admin.UserName) != null;
        if (!adminExists)
        {
            await _userManager.CreateAsync(RequiredData.Admin, RequiredData.AdminPassword);
            await _userManager.AddToRoleAsync(RequiredData.Admin, "Admin");
        }
    }
}