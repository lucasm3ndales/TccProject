using System.Net.WebSockets;

namespace CarbonBlockchain.Services.WebSocketHosted;

public class WebSocketService: BackgroundService, IWebSocketService
{
    protected override Task ExecuteAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task ConnectAsync(WebSocket webSocket, object? message, Func<string?, Task> onMessage)
    {
        throw new NotImplementedException();
    }
}