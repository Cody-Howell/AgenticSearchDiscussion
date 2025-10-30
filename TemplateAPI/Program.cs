using TemplateAPI;
using TemplateAPI.Classes;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.IO;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

var env = Env.Load();
var AI_TOKEN = env.Where(k => k .Key== "AI_TOKEN").FirstOrDefault().Value ?? throw new InvalidOperationException("AI_TOKEN environment variable is not set.");
var connString = builder.Configuration["DOTNET_DATABASE_STRING"] ?? throw new InvalidOperationException("Connection string for database not found.");
Console.WriteLine("Connection String: " + connString);
var dbConnector = new DatabaseConnector(connString);
builder.Services.AddSingleton<IDbConnection>(provider => {
    return dbConnector.ConnectWithRetry();
});

builder.Services.AddSingleton<DBService>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/api/health", () => "Hello");

// Add new endpoints here
/*
curl -X POST https://ai-snow.reindeer-pinecone.ts.net/api/chat/completions \
  -H "Authorization: Bearer {{$dotenv AI_TOKEN}}" \
  -H "Content-Type: application/json" \
  -d '{
  "model": "gpt-oss-120b", 
  "messages": [
    {
      "role": "user", 
      "content": "Say anything"
    }
  ]
}'
*/

app.MapPost("/api/chat", async (HttpContext httpContext, IHttpClientFactory httpClientFactory) =>
{
  // Read the raw string body
  string message;
  using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
  {
    message = await reader.ReadToEndAsync();
  }

    if (string.IsNullOrWhiteSpace(message)) {
        httpContext.Response.StatusCode = 400;
        await httpContext.Response.WriteAsync("Request body must contain a non-empty string message.");
        return;
    }
  
  Console.WriteLine("Received message: " + message);

  // Build payload
  var payload = new
  {
    model = "gpt-oss-120b",
    messages = new[] { new { role = "user", content = message } }
  };

  var json = JsonSerializer.Serialize(payload);

  var client = httpClientFactory.CreateClient();
  var req = new HttpRequestMessage(HttpMethod.Post, "https://ai-snow.reindeer-pinecone.ts.net/api/chat/completions");
  req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AI_TOKEN);
  req.Content = new StringContent(json, Encoding.UTF8, "application/json");

  HttpResponseMessage resp;
  try
  {
    resp = await client.SendAsync(req);
  }
  catch (Exception ex)
  {
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

app.MapFallbackToFile("index.html");

app.Run();
