using System.Net.WebSockets;
using CarbonCertifier.Services.WebSocketHosted.Dtos;

namespace CarbonCertifier.Services.WebSocketHosted;

public interface IWebSocketHostedServerService
{
    Task ConnectAsync(WebSocket webSocket, WebSocketMessageDto? message, Func<object?, Task> onMessage);
    Task SendWebSocketMessageAsync(object message);
}