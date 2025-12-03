using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickType;
using System.Linq;
using System.Text;
using TemplateAPI.Function;
using TemplateAPI.Services;

namespace TemplateAPI.Endpoints;

public static class TodoEndpoint {
    public static WebApplication MapTodoEndpoints(this WebApplication app) {
        app.MapGet("/api/todo/{id}", (int id, [FromServices] TodoService service) => {
            return service.GetTodoItems(id);
        });
        app.MapGet("/api/todo/{id}/oldest", (int id, [FromServices] TodoService service) => {
            return service.GetOldestItem(id);
        });

        app.MapPost("/api/todo/{id}", async ([FromServices] TodoService service, int id, string item) => {
            await service.AddTodoItem(id, item);
            return Results.Ok();
        });

        app.MapPost("/api/todo/{id}/break", async (int id, HttpContext httpContext, TodoService todos, IChatService chatService) => {
            string message;
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8)) {
                message = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(message)) {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Request body must contain a non-empty string message.");
                return;
            }

            Console.WriteLine("Received message: " + message);

            string sysMessage = TemplateAPI.Classes.SystemMessages.TodoBreakdown;
            List<UserMessage> messages = new List<UserMessage>() {
                new UserMessage { Role = "system", Content = sysMessage} ,
                new UserMessage { Role = "user", Content = message }
            };

            var functionTest = TemplateAPI.Classes.TodoTools.GetTodoListTool();

            AiResponse aiResp;
            try {
                aiResp = await chatService.ProcessTodoBreakdownAsync(
                    messages,
                    functionTest,
                    async (item) => await todos.AddTodoItem(id, item)
                );
            } catch (Exception ex) {
                httpContext.Response.StatusCode = 502;
                await httpContext.Response.WriteAsync($"Error calling AI server: {ex.Message}");
                return;
            }

            var respJson = JsonConvert.SerializeObject(aiResp);
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(respJson);
        });

        app.MapDelete("/api/todo/{id}", ([FromServices] TodoService service, int id, Guid? item) => {
            if (item is not null) {
                service.DeleteTodoItem(id, (Guid)item);
            } else {
                service.DeleteOldestItem(id);
            }
            return Results.Ok();
        });

        return app;
    }
}