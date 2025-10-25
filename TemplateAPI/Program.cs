using TemplateAPI;
using TemplateAPI.Classes;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration["DOTNET_DATABASE_STRING"] ?? throw new InvalidOperationException("Connection string for database not found.");
Console.WriteLine("Connection String: " + connString);
var dbConnector = new DatabaseConnector(connString);
builder.Services.AddSingleton<IDbConnection>(provider => {
    return dbConnector.ConnectWithRetry();
});

builder.Services.AddSingleton<DBService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/api/health", () => "Hello");

// Add new endpoints here

app.MapFallbackToFile("index.html");

app.Run();
