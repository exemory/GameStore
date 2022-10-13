using Business.Extensions;
using Business.Options;
using Data.Extensions;
using Serilog;
using WebApi.Extensions;
using WebApi.Middlewares;
using WebApi.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) =>
{
    config.ReadFrom.Configuration(builder.Configuration);
});

var connectionStrings = builder.Configuration
    .GetSection(nameof(ConnectionStrings))
    .Get<ConnectionStrings>();

builder.Services.AddDataLayer(c =>
{
    c.ConnectionString = connectionStrings.DefaultConnection;
    c.EnableSensitiveDataLogging = builder.Environment.IsDevelopment();
});

var storageConfiguration = builder.Configuration.GetSection(nameof(StorageOptions));
var jwtConfiguration = builder.Configuration.GetSection(nameof(JwtOptions));
builder.Services.AddBusinessLayer(storageConfiguration, jwtConfiguration);

var jwtOptions = jwtConfiguration.Get<JwtOptions>();
builder.Services.AddWebApi(jwtOptions);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors(b =>
    b.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
);

app.UseResponseCaching();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<IpAddressLoggerMiddleware>();
app.UseMiddleware<PerformanceLoggerMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await app.InitializeDb();
app.InitializeStorageFolders();

app.Run();