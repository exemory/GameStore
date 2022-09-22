using Data.Interfaces;
using Data.Options;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, Action<DataLayerOptions> configAction)
    {
        var config = new DataLayerOptions();
        configAction(config);
        
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
            options.UseSqlServer(config.ConnectionString);

            if (config.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}