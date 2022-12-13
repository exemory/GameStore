using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebApi.IntegrationTests;

public class TestingWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(ConfigureServices);
    }
    
    private static void ConfigureServices(IServiceCollection services)
    {
        services.RemoveAll<GameStoreContext>();
        services.RemoveAll<DbContextOptions>();
        services.RemoveAll<DbContextOptions<GameStoreContext>>();

        var databaseName = Guid.NewGuid().ToString();

        services.AddDbContext<GameStoreContext>(o =>
        {
            o.UseInMemoryDatabase(databaseName);
            o.EnableSensitiveDataLogging();
        });
    }
}