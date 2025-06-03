using CarbonBlockchain.Services.WebSocketHostedClient.Dtos;

namespace CarbonBlockchain.Services.WebSocketHostedClient;

public interface IWebSocketHostedClientService
{
    Task SendMessageAsync(WebSocketMessageDto message);
}