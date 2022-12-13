using AutoFixture;
using Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace WebApi.IntegrationTests;

public abstract class IntegrationTests : IClassFixture<TestingWebAppFactory>
{
    protected readonly Fixture Fixture = new();

    protected readonly TestingWebAppFactory AppFactory;
    protected readonly IServiceScope ServiceScope;
    protected readonly GameStoreContext DbContext;

    protected IntegrationTests(TestingWebAppFactory appFactory)
    {
        AppFactory = appFactory;
        ServiceScope = appFactory.Services.CreateScope();
        DbContext = ServiceScope.ServiceProvider.GetRequiredService<GameStoreContext>();
    }
}