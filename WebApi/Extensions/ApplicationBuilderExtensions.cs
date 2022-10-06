using Business.Interfaces;
using Business.Options;
using Microsoft.Extensions.Options;

namespace WebApi.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Game Store API v1");
        });

        return app;
    }

    public static async Task InitializeDb(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await dbInitializer.Initialize();
    }
    
    public static IApplicationBuilder InitializeStorageFolders(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var storageOptions = scope.ServiceProvider.GetRequiredService<IOptions<StorageOptions>>().Value;

        Directory.CreateDirectory(storageOptions.GameImagesFolderPath);

        return app;
    }
}