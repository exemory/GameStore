using Business.Extensions;
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

await app.InitializeDb();

app.Run();