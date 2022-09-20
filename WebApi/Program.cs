using Business.Extensions;
using Business.Interfaces;
using Data.Extensions;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var enableSensitiveDataLogging = builder.Environment.IsDevelopment();
builder.Services.AddDataLayer(connectionString, enableSensitiveDataLogging);

builder.Services.AddBusinessLayer();

builder.Services.AddWebApi();

var app = builder.Build();

app.UseWebApiPipeline();

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.Initialize();
}

app.Run();