using System.Net.WebSockets;
using CarbonCertifier.Services.WebSocketHostedServer.Dtos;

namespace CarbonCertifier.Services.WebSocketHostedServer;

public interface IWebSocketHostedServerService
{
    Task ConnectAsync(WebSocket webSocket, WebSocketMessageDto? message, Func<string, Task> onMessage);
    Task SendWebSocketMessageAsync(object message);
}