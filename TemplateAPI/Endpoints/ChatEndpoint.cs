using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using QuickType;

namespace TemplateAPI.Endpoints;

public static class ChatEndpoint {
    public static void MapChatEndpoint(this WebApplication app) {
        string AI_TOKEN = app.Configuration["AI_TOKEN"] ?? throw new InvalidOperationException("AI_TOKEN environment variable is not set.");

        app.MapPost("/api/chat", async (HttpContext httpContext, IHttpClientFactory httpClientFactory) => {
            // Read the raw string body
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

            // Build payload
            var payload = new AiRequest() {
                Model = "gpt-oss-120b",
                Messages = [new UserMessage { Role = "user", Content = message }]
            };

            var json = JsonConvert.SerializeObject(payload);

            Console.WriteLine("JSON object is: " + json);

            var client = httpClientFactory.CreateClient();
            var req = new HttpRequestMessage(HttpMethod.Post, "https://ai-snow.reindeer-pinecone.ts.net/api/chat/completions");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AI_TOKEN);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage resp;
            try {
                resp = await client.SendAsync(req);
            } catch (Exception ex) {
                httpContext.Response.StatusCode = 502;
                await httpContext.Response.WriteAsync($"Error calling AI server: {ex.Message}");
                return;
            }

            var respText = await resp.Content.ReadAsStringAsync();
            AiResponse response = AiResponse.FromJson(respText);
            var contentType = resp.Content.Headers.ContentType?.ToString() ?? "application/json";

            // Forward status code, content-type, and body
            httpContext.Response.StatusCode = (int)resp.StatusCode;
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.WriteAsync(respText);
        });
    }
}
