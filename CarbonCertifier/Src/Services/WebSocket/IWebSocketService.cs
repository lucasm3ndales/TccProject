using System.Net.WebSockets;
using CarbonCertifier.Services.Wss.Dtos;

namespace CarbonCertifier.Services.Wss;

public interface IWebSocketService
{
    Task ConnectAsync(WebSocket webSocket, object? message, Func<WebSocketMessageDto?, Task> onMessage);
}