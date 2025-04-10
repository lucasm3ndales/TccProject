using System.Net.WebSockets;

namespace CarbonCertifier.Services.WebSocketHosted;

public interface IWebSocketHostedServerService
{
    Task ConnectAsync(WebSocket webSocket, object? message, Func<string?, Task> onMessage);
}