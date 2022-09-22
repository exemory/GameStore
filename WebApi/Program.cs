using Business.Extensions;
using Data.Extensions;
using WebApi.Extensions;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var enableSensitiveDataLogging = builder.Environment.IsDevelopment();
builder.Services.AddDataLayer(connectionString, enableSensitiveDataLogging);

builder.Services.AddBusinessLayer();

builder.Services.AddWebApi();

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

app.UseResponseCaching();

app.UseRouting();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await app.InitializeDb();

app.Run();