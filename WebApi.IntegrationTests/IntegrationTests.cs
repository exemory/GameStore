using AutoFixture;
using Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebApi.IntegrationTests;

public abstract class IntegrationTests
{
    protected readonly HttpClient TestClient;
    protected readonly Fixture Fixture = new();

    protected IntegrationTests()
    {
        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(ConfigureServices);
            });

        TestClient = appFactory.CreateClient();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.RemoveAll<GameStoreContext>();
        services.RemoveAll<DbContextOptions>();
        services.RemoveAll<DbContextOptions<GameStoreContext>>();

        var s = Fixture.Create<string>();

        services.AddDbContext<GameStoreContext>(o =>
        {
            o.UseInMemoryDatabase(s);
            o.EnableSensitiveDataLogging();
        });

        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();

        dbContext.Database.EnsureCreated();
    }
}