using WebApi.Middlewares;

namespace WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseWebApiPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Game Store API v1");
            });
        }
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseResponseCaching();

        app.UseRouting();

        app.UseMiddleware<ErrorHandlerMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }
}