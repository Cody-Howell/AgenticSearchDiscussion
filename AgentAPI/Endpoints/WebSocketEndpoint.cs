using System.Net.WebSockets;
using TemplateAPI.Services;

namespace TemplateAPI.Endpoints;

public static class WebSocketEndpoint {
    public static WebApplication MapWebSocketEndpoints(this WebApplication app) {
        app.Map("/api/ws/{id:int}", async (HttpContext context, int id, WebSocketService wsService) => {
            if (context.WebSockets.IsWebSocketRequest) {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                
                wsService.RegisterSocket(id, webSocket);

                await wsService.KeepAliveAsync(id, webSocket);
            } else {
                context.Response.StatusCode = 400;
            }
        });

        return app;
    }
}
