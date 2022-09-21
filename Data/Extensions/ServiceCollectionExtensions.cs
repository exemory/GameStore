using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString,
        bool enableSensitiveDataLogging)
    {
        services.Scan(s =>
        {
            s.FromAssembliesOf(typeof(IRepository<>))
                .AddClasses(classes =>
                    classes.AssignableTo(typeof(Repository<>))
                        .Where(t => t != typeof(Repository<>)))
                .AsMatchingInterface()
                .WithScopedLifetime();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddDbContext<GameStoreContext>(options =>
        {
            options.UseSqlServer(connectionString);

            if (enableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}