using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuickType;
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

            string sysMessage = """
            Take in an input from the user. 
            Break down the message into discrete chunks for an AI agent to read from, one at a time sequentially. 
            Call the function until you've parsed the entire message and have no more items.
            Once done, provide a message to the user summarizing what you've done.
            """;
            List<UserMessage> messages = new List<UserMessage>() {
                new UserMessage { Role = "system", Content = sysMessage} ,
                new UserMessage { Role = "user", Content = message }
            };

            var functionTest = new Tool {
                Type = "function",
                Function = new CalledFunction {
                    Name = "todo_list",
                    Description = "Add to the todo list for an AI agent to stay focused.",
                    Parameters = new Parameters {
                        Type = "string",
                        Properties = new Dictionary<string, ParameterDescription> {
                            ["item"] = new ParameterDescription {
                                Type = "string",
                                Description = "The todo list item to add to the array."
                            },
                        },
                        ParametersRequired = ["item"]
                    }
                }
            };

            AiResponse aiResp;
            try {
                // while (true) {
                    aiResp = await chatService.SendMessageAsync([.. messages], [functionTest]);
                    if (aiResp.Choices[0].FinishReason != "tool_calls") {
                        
                    }
                // }
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