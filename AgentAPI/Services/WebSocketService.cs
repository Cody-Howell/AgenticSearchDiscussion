using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace TemplateAPI.Services;

public class WebSocketService {
    private readonly ConcurrentDictionary<int, List<WebSocket>> _connections = new();
    private readonly object _lock = new();

    public void RegisterSocket(int id, WebSocket socket) {
        lock (_lock) {
            if (!_connections.ContainsKey(id)) {
                _connections[id] = new List<WebSocket>();
            }
            _connections[id].Add(socket);
        }
        Console.WriteLine($"WebSocket registered for ID: {id}. Total connections for this ID: {_connections[id].Count}");
    }

    public void UnregisterSocket(int id, WebSocket socket) {
        lock (_lock) {
            if (_connections.ContainsKey(id)) {
                _connections[id].Remove(socket);
                if (_connections[id].Count == 0) {
                    _connections.TryRemove(id, out _);
                }
            }
        }
        Console.WriteLine($"WebSocket unregistered for ID: {id}");
    }

    public async Task SendMessageAsync(int id, string message) {
        List<WebSocket> socketsToRemove = new();
        List<WebSocket>? sockets;

        lock (_lock) {
            if (!_connections.TryGetValue(id, out sockets) || sockets.Count == 0) {
                Console.WriteLine($"No active WebSocket connections for ID: {id}");
                return;
            }
            sockets = new List<WebSocket>(sockets); // Create a copy to iterate safely
        }

        var messageBytes = Encoding.UTF8.GetBytes(message);
        var buffer = new ArraySegment<byte>(messageBytes);

        foreach (var socket in sockets) {
            if (socket.State == WebSocketState.Open) {
                try {
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                } catch (Exception ex) {
                    Console.WriteLine($"Error sending message to socket for ID {id}: {ex.Message}");
                    socketsToRemove.Add(socket);
                }
            } else {
                socketsToRemove.Add(socket);
            }
        }

        // Clean up closed sockets
        if (socketsToRemove.Count > 0) {
            lock (_lock) {
                foreach (var socket in socketsToRemove) {
                    UnregisterSocket(id, socket);
                }
            }
        }
    }

    public async Task KeepAliveAsync(int id, WebSocket socket) {
        var buffer = new byte[1024 * 4];
        
        try {
            while (socket.State == WebSocketState.Open) {
                var result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );

                if (result.MessageType == WebSocketMessageType.Close) {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing",
                        CancellationToken.None
                    );
                    break;
                }
                // Ignore incoming messages - we only send
            }
        } catch (Exception ex) {
            Console.WriteLine($"WebSocket error for ID {id}: {ex.Message}");
        } finally {
            UnregisterSocket(id, socket);
        }
    }
}
