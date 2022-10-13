using Business.Interfaces;
using Business.Options;
using Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        services.AddScoped<IDbInitializer, DbInitializer>();

        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IPlatformTypeService, PlatformTypeService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped<ISession, Session>();

        services.AddAutoMapper(typeof(AutomapperProfile));
        
        return services;
    }
}