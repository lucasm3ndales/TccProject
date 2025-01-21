using System.Net.WebSockets;

namespace CarbonCertifier.Services.WebSocketHosted;

public interface IWebSocketHostedService
{
    Task ConnectAsync(WebSocket webSocket, object? message, Func<string?, Task> onMessage);
}