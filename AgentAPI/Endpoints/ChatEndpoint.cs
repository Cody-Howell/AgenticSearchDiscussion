using Newtonsoft.Json;
using QuickType;
using System.Text;
using TemplateAPI.Endpoints;
using TemplateAPI.Function;
using TemplateAPI.Classes;

namespace TemplateAPI.Endpoints;

public static class ChatEndpoint {
    public static WebApplication MapChatEndpoints(this WebApplication app) {    
        app.MapGet("/api/chats/{chatId:int}/messages", async (int chatId, DBService db) => {
            var messages = await db.GetMessagesAsync(chatId);
            return Results.Ok(messages);
        });
        
        app.MapPost("/api/chats/create", async (DBService db) => {
            var chatId = await db.CreateChatAsync();
            return Results.Created($"/api/chats/{chatId}", new { id = chatId });
        });
        
        app.MapPost("/api/chats/{chatId:int}/messages", async (int chatId, string message, DBService db) => {
            DBMessage m = new () {ChatId = chatId, MessageText = message, Role = "user", Type = "message"};
            await db.AddMessageAsync(m);
            return Results.Created();
        });
        
        app.MapDelete("/api/chats/{chatId:int}/messages/{id:int}", async (int chatId, int id, DBService db) => {
            var deleted = await db.DeleteMessageAsync(id, chatId);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}