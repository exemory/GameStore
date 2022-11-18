using Business.Interfaces;
using Business.Options;
using Business.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ServiceNameEnding = "Service";

    public static IServiceCollection AddBusinessLayer(this IServiceCollection services,
        IConfiguration storageConfiguration, IConfiguration jwtConfiguration)
    {
        services.Configure<StorageOptions>(storageConfiguration);
        services.Configure<JwtOptions>(jwtConfiguration);

        services.AddScoped<IDbInitializer, DbInitializer>();

        services.Scan(s =>
        {
            s.FromAssembliesOf(typeof(IAuthenticationService))
                .AddClasses(classes =>
                    classes.Where(t => t.Name.EndsWith(ServiceNameEnding)))
                .AsMatchingInterface()
                .WithScopedLifetime();
        });

        services.AddScoped<ISession, Session>();

        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddAutoMapper(typeof(AutomapperProfile));

        return services;
    }
}