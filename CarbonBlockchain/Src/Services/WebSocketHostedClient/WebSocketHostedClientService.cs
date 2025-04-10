using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CarbonBlockchain.Services.WebSocketHosted.Dtos;

namespace CarbonBlockchain.Services.WebSocketHosted;

// TODO: Implementar cliente web socket
public class WebSocketHostedClientService(IConfiguration configuration): BackgroundService, IWebSocketHostedClientService
{
    private readonly TimeSpan _reconnectInterval = TimeSpan.FromSeconds(5);
    private readonly string? _webSocketConnectionUrl = configuration.GetConnectionString("WebSocketConnection");

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {   

    }

    private async Task SendMessageAsync(WebSocket webSocket, object? message)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            var json = JsonSerializer.Serialize(message, options);
            var buffer = Encoding.UTF8.GetBytes(json);

            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message through WebSocket: {ex.Message}");
        }
    }

    private async Task HandleReceivedMessageAsync(WebSocket webSocket, string message, Func<string?, Task> onMessage)
    {
        try
        {
            WebSocketMessageDto? responseDto = null;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonMessage = JsonSerializer.Deserialize<WebSocketMessageDto>(message, options);

            if (jsonMessage == null || jsonMessage.Message == null)
            {
                responseDto = new WebSocketMessageDto(
                    400,
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    "Message cannot be empty.");
            }

            if (jsonMessage != null && jsonMessage.Message != null)
            {
                responseDto = new WebSocketMessageDto(
                    200,
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    "Message received successfully!");

                var data = JsonSerializer.Serialize(jsonMessage.Message);

                await onMessage(data);
            }

            await SendMessageAsync(webSocket, responseDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to handle web socket received message: {ex.Message}");

        }

    }
}