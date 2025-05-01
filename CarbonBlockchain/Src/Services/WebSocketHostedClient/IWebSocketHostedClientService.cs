using System.Net.WebSockets;
using CarbonBlockchain.Services.WebSocketHosted.Dtos;

namespace CarbonBlockchain.Services.WebSocketHosted;

public interface IWebSocketHostedClientService
{
    Task SendMessageAsync(WebSocketMessageDto message);
}