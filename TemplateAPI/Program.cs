using TemplateAPI;
using TemplateAPI.Classes;
using System.Data;
using TemplateAPI.Endpoints;
using TemplateAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var AI_TOKEN = builder.Configuration["AI_TOKEN"] ?? throw new InvalidOperationException("AI_TOKEN environment variable is not set.");
var connString = builder.Configuration["DOTNET_DATABASE_STRING"] ?? throw new InvalidOperationException("Connection string for database not found.");
Console.WriteLine("Connection String: " + connString);
var dbConnector = new DatabaseConnector(connString);
builder.Services.AddSingleton<IDbConnection>(provider => {
    return dbConnector.ConnectWithRetry();
});

builder.Services.AddSingleton<DBService>();
builder.Services.AddSingleton<TodoService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/api/health", () => "Hello");
app.MapChatEndpoints()
    .MapTodoEndpoints()
    .MapFileEndpoints();


app.MapFallbackToFile("index.html");

app.Run();
