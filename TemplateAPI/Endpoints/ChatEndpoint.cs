using QuickType;
using System.Text;
using Newtonsoft.Json;
using TemplateAPI.Endpoints;
using TemplateAPI.Function;

namespace TemplateAPI.Endpoints;

public static class ChatEndpoint {
    public static WebApplication MapChatEndpoints(this WebApplication app) {
        app.MapPost("/api/chat", async (HttpContext httpContext, IChatService chatService) => {
            // Read the raw plaintext body
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

            var userMessage = new UserMessage { Role = "user", Content = message };
            var functionTest = new Tool
                    {
                        Type = "function",
                        Function = new CalledFunction
                        {
                            Name = "search_tool",
                            Description = "A tool to search the web for relevant information.",
                            Parameters = new Parameters
                            {
                                Type = "object",
                                Properties = new Dictionary<string, ParameterDescription>
                                {
                                    ["summary"] = new ParameterDescription
                                    {
                                        Type = "string",
                                        Description = "The search query provided by the user."
                                    },
                                    ["maxResults"] = new ParameterDescription
                                    {
                                        Type = "integer",
                                        Description = "The maximum number of results to return."
                                    }
                                },
                                ParametersRequired = new[] { "summary" }
                            }
                        }
                    };

            AiResponse aiResp;
            try {
                aiResp = await chatService.SendMessageAsync([userMessage], [functionTest]);
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

        return app;
    }
}