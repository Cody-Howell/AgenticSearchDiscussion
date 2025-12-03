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
            Console.WriteLine("Recieved todo item: ", item);
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

        // Answers all existing todo items sequentially using the ReadTodos system prompt.
        // No functions/tools are allowed; runs in a loop until all items are processed.
        app.MapPost("/api/todo/{id}/answer-all", async (int id, HttpContext httpContext, TodoService todos, IChatService chatService, Services.WebSocketService wsService) => {
            try {
                var existing = todos.GetTodoItems(id)?.ToList() ?? new List<TemplateAPI.Classes.TodoItem>();
                if (existing.Count == 0) {
                    httpContext.Response.StatusCode = 200;
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { count = 0, answers = new string[0] }));
                    return;
                }

                string sysMessage = TemplateAPI.Classes.SystemMessages.ReadTodos;
                var messages = new List<UserMessage> {
                    new UserMessage { Role = "system", Content = sysMessage }
                };

                var answers = new List<string>();

                // Process each todo item text as an independent task with no tools.
                foreach (var item in existing) {
                    if (string.IsNullOrWhiteSpace(item.Text)) {
                        answers.Add(string.Empty);
                        continue;
                    }

                    var perItemMessages = new List<UserMessage>(messages) {
                        new UserMessage { Role = "user", Content = item.Text }
                    };

                    // Emit a WebSocket tool-call styled event representing the incoming user task
                    var userToolCallJson = JsonConvert.SerializeObject(new [] {
                        new {
                            type = "function",
                            function = new {
                                name = "user",
                                arguments = JsonConvert.SerializeObject(new { task = item.Text })
                            },
                            id = Guid.NewGuid().ToString()
                        }
                    });
                    var userToolCallMsg = new TemplateAPI.Classes.DBMessage {
                        ChatId = id,
                        Role = "assistant",
                        Type = "tool_call",
                        MessageText = userToolCallJson
                    };
                    await wsService.SendMessageAsync(id, JsonConvert.SerializeObject(new {
                        type = "chat_message",
                        content = userToolCallMsg
                    }));

                    AiResponse aiResp = await chatService.SendMessageAsync([.. perItemMessages], null);
                    string answer = string.Empty;
                    if (aiResp?.Choices != null && aiResp.Choices.Length > 0) {
                        answer = aiResp.Choices[0].Message?.Content ?? string.Empty;
                    }
                    answers.Add(answer);

                    // Emit a WebSocket message with the AI's answer
                    if (!string.IsNullOrWhiteSpace(answer)) {
                        var aiMsg = new TemplateAPI.Classes.DBMessage {
                            ChatId = id,
                            Role = "assistant",
                            Type = "message",
                            MessageText = answer
                        };
                        await wsService.SendMessageAsync(id, JsonConvert.SerializeObject(new {
                            type = "chat_message",
                            content = aiMsg
                        }));
                    }
                }

                var respJson = JsonConvert.SerializeObject(new { count = answers.Count, answers });
                httpContext.Response.StatusCode = 200;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(respJson);
            } catch (Exception ex) {
                httpContext.Response.StatusCode = 502;
                await httpContext.Response.WriteAsync($"Error processing todo answers: {ex.Message}");
            }
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