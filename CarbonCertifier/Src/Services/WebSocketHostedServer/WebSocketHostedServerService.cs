using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CarbonCertifier.Services.WebSocketHosted.Dtos;

namespace CarbonCertifier.Services.WebSocketHosted;

public class WebSocketHostedServerService(IConfiguration configuration): BackgroundService, IWebSocketHostedServerService
{
    private readonly Dictionary<Guid, WebSocket> _clients = new();
    private readonly byte[] _buffer = new byte[1024 * 4];
    private readonly TimeSpan _reconnectInterval = TimeSpan.FromSeconds(5);
    private readonly string? _webSocketConnectionUrl = configuration.GetConnectionString("WebSocketConnection");

    protected override Task ExecuteAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task ConnectAsync(WebSocket webSocket, object? message, Func<string?, Task> onMessage)
    {
        var clientId = Guid.NewGuid();

        _clients[clientId] = webSocket;

        Console.WriteLine($"Client {clientId} start connection.");

        try
        {

            if (message != null)
            {
                await SendMessageAsync(webSocket, message);
            }

            while (!webSocket.CloseStatus.HasValue)
            {
                var response = await webSocket.ReceiveAsync(new ArraySegment<byte>(_buffer), CancellationToken.None);

                if (response.MessageType == WebSocketMessageType.Text)
                {
                    var receivedMessage = Encoding.UTF8.GetString(_buffer, 0, response.Count);

                    await HandleReceivedMessageAsync(webSocket, receivedMessage, onMessage);
                }

                if (response.MessageType == WebSocketMessageType.Close && webSocket.CloseStatus.HasValue)
                {
                    await webSocket.CloseAsync(webSocket.CloseStatus.Value, webSocket.CloseStatusDescription, CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error with WebSocket connection for client {clientId}: {ex.Message}");
        }
        finally
        {
            _clients.Remove(clientId);
            await HandleClientDisconnectionAsync(clientId);
        }
    }

    private async Task HandleClientDisconnectionAsync(Guid clientId)
    {
        Console.WriteLine($"Client {clientId} disconnected. Attempting to reconnect...");

        while (true)
        {
            try
            {
                var newSocket = new ClientWebSocket();

                await newSocket.ConnectAsync(new Uri(_webSocketConnectionUrl!), CancellationToken.None);

                Console.WriteLine($"Client {clientId} reconnected successfully.");

                _clients[clientId] = newSocket;

                var responseDto = new WebSocketMessageDto(
                    200,
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    "Reconnected successfully.");

                await SendMessageAsync(newSocket, responseDto);

                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error to reconnect to client {clientId}: {ex.Message}. Retrying in {_reconnectInterval.Seconds} seconds...");
                await Task.Delay(_reconnectInterval);
            }
        }
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
