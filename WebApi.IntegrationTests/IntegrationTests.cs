using AutoFixture;
using Business.Interfaces;
using Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WebApi.IntegrationTests;

public abstract class IntegrationTests : IClassFixture<TestingWebAppFactory>
{
    protected readonly HttpClient TestClient;
    protected readonly Fixture Fixture = new();

    protected IntegrationTests(TestingWebAppFactory appFactory)
    {
        using var scope = appFactory.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize().GetAwaiter().GetResult();

        TestClient = appFactory.CreateClient();
    }
}