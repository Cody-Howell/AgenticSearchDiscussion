using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickType;
using System.Text;
using System.Linq;
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
            Take in an input from the user. The first message is the 
            Break down the message into discrete chunks for an AI agent to read from, one at a time sequentially. 
            Call the function until you've parsed the entire message and have no more items. See previous user messages 
            for the result of your tool calls. Don't duplicate items.
            Once done, provide a message to the user saying that you're done. Make it as short as possible. 
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
                        Type = "object",
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
            int count = 0;
            try {
                while (true) {
                    Console.WriteLine("\n\n\n------Starting New Iteration--------");
                    if (count < 8) {
                        aiResp = await chatService.SendMessageAsync([.. messages], [functionTest]);
                    } else {
                        aiResp = await chatService.SendMessageAsync([.. messages], []);
                    }

                    if (aiResp?.Choices == null || aiResp.Choices.Length == 0) break;
                    if (aiResp.Choices[0].FinishReason != "tool_calls") break;

                    var toolCall = aiResp.Choices[0].ToolCalls?.FirstOrDefault();
                    if (toolCall == null || toolCall.Function == null) break;

                    var argsJson = toolCall.Function.Arguments ?? string.Empty;
                    string? itemArg = null;

                    // Console.WriteLine(Serialize.ToJson(aiResp));

                    // If the tool call is an assistant instruction, add it as an assistant message
                    if (toolCall.Function.Name == "assistant_instruction") {
                        string? instruction = null;
                        try {
                            var dictInst = JsonConvert.DeserializeObject<Dictionary<string, string>>(argsJson);
                            if (dictInst != null && dictInst.TryGetValue("instruction", out var v)) instruction = v;
                        } catch { }

                        if (instruction == null) {
                            try {
                                var joInst = JObject.Parse(argsJson);
                                instruction = joInst["instruction"]?.ToString();
                            } catch { }
                        }

                        if (!string.IsNullOrWhiteSpace(instruction)) {
                            messages.Add(new UserMessage { Role = "assistant", Content = instruction });
                            // continue to next iteration so instruction can influence subsequent calls
                            count++;
                            continue;
                        }
                        // if we couldn't extract an instruction, fall through and treat as raw assistant content
                    }
                    try {
                        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(argsJson);
                        if (dict != null && dict.TryGetValue("item", out var v)) itemArg = v;
                    } catch { }

                    if (itemArg == null) {
                        try {
                            var jo = JObject.Parse(argsJson);
                            itemArg = jo["item"]?.ToString();
                        } catch { }
                    }

                    if (string.IsNullOrWhiteSpace(itemArg)) {
                        // If we couldn't extract an item, add the raw args as an assistant message and stop looping
                        messages.Add(new UserMessage { Role = "assistant", Content = argsJson });
                        break;
                    }

                    // Call the TodoService to add the item
                    todos.AddTodoItem(id, itemArg);



                    // Append the added item as an user message so the AI knows the action result
                    messages.Add(new UserMessage { Role = "tool", Content = itemArg });
                    foreach (var item in messages) {
                        Console.WriteLine($"Role: {item.Role}, Content: {item.Content}");
                    }
                    count++;
                }
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