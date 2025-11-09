using System.Text;
using TemplateAPI.Function;

namespace TemplateAPI.Endpoints;

public static class ChatEndpoint {
    public static void MapChatEndpoints(this WebApplication app) {
        string AI_TOKEN = app.Configuration["AI_TOKEN"] ?? throw new InvalidOperationException("AI_TOKEN environment variable is not set.");
        string serverUrl = app.Configuration["AI_SERVER_URL"] ?? throw new InvalidOperationException("AI_SERVER_URL environment variable is not set.");

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

            HttpResponseMessage resp;
            try {
                resp = await ChatHelper.SendMessageToAIAsync(message, AI_TOKEN, serverUrl, httpClientFactory);
            } catch (Exception ex) {
                httpContext.Response.StatusCode = 502;
                await httpContext.Response.WriteAsync($"Error calling AI server: {ex.Message}");
                return;
            }

            var respText = await resp.Content.ReadAsStringAsync();
            var contentType = resp.Content.Headers.ContentType?.ToString() ?? "application/json";

            // Forward status code, content-type, and body
            httpContext.Response.StatusCode = (int)resp.StatusCode;
            httpContext.Response.ContentType = contentType;
            await httpContext.Response.WriteAsync(respText);
        });
    }
}
