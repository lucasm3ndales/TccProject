namespace CarbonBlockchain.Services.WebSocketHostedClient;

public interface IWebSocketHostedClientService
{
    Task SendWebSocketMessageAsync(object message);
}