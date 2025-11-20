using Microsoft.AspNetCore.Mvc;
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

        app.MapPost("/api/todo/{id}", ([FromServices] TodoService service, int id, string item) => {
            service.AddTodoItem(id, item);
            return Results.Ok();
        });

        app.MapPost("/api/todo/{id}/", (int id, TodoService todos, IChatService chatService) => {
            // Placeholder: endpoint now receives IChatService via DI for future use
            return Results.Ok();
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