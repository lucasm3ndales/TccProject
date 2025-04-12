using System.Net.WebSockets;

namespace CarbonBlockchain.Services.WebSocketHosted;

public interface IWebSocketHostedClientService
{
    Task SendMessageAsync(object message);
}