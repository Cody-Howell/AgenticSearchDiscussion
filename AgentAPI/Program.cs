using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using TemplateAPI;
using TemplateAPI.Classes;
using TemplateAPI.Endpoints;
using TemplateAPI.Function;
using TemplateAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var AI_TOKEN = builder.Configuration["AI_TOKEN"] ?? throw new InvalidOperationException("AI_TOKEN environment variable is not set.");
var connString = builder.Configuration["DOTNET_DATABASE_STRING"] ?? throw new InvalidOperationException("Connection string for database not found.");
Console.WriteLine("Connection String: " + connString);
var dbConnector = new DatabaseConnector(connString);
builder.Services.AddSingleton<IDbConnection>(provider => {
    return dbConnector.ConnectWithRetry();
});

builder.Services.AddSingleton<DBService>();
builder.Services.AddSingleton<TodoService>();
builder.Services.AddSingleton<WebSocketService>();
// HTTP client and Chat service registration
builder.Services.AddHttpClient();
var aiServerUrl = builder.Configuration["AI_SERVER_URL"] ?? throw new InvalidOperationException("AI_SERVER_URL environment variable is not set.");
builder.Services.AddSingleton(new ChatServiceConfig { ServerUrl = aiServerUrl, AiToken = AI_TOKEN });
builder.Services.AddSingleton<IChatService, ChatService>();

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
// })
// .AddCookie()
// .AddOpenIdConnect(options =>
// {
//     var oidcConfig = builder.Configuration.GetSection("OpenIDConnectSettings");

//     options.Authority = oidcConfig["Authority"];
//     options.ClientId = oidcConfig["ClientId"];
//     options.ClientSecret = oidcConfig["ClientSecret"];

//     options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     options.ResponseType = OpenIdConnectResponseType.Code;

//     options.SaveTokens = true;
//     options.GetClaimsFromUserInfoEndpoint = true;

//     options.MapInboundClaims = false;
//     options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
//     options.TokenValidationParameters.RoleClaimType = "roles";
// });

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseWebSockets();
// app.UseAuthentication();
// app.UseAuthorization();

app.MapGet("/api/health", () => "Hello");
app.MapChatEndpoints()
    .MapTodoEndpoints()
    .MapFileEndpoints()
    .MapWebSocketEndpoints();

app.MapGet("/api/auth", () => "Authorized").RequireAuthorization();

app.MapFallbackToFile("index.html");

app.Run();