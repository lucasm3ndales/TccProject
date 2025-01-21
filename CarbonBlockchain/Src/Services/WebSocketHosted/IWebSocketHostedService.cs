using System.Net.WebSockets;

namespace CarbonBlockchain.Services.WebSocketHosted;

public interface IWebSocketService
{
    Task ConnectAsync(WebSocket webSocket, object? message, Func<string?, Task> onMessage);

}