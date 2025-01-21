using System.Net.WebSockets;

namespace CarbonCertifier.Services.Wss;

public interface IWebSocketService
{
    Task ConnectAsync(WebSocket webSocket, object? message, Func<string?, Task> onMessage);
}