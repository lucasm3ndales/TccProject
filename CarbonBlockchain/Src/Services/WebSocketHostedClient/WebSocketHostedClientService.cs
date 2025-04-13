using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using CarbonBlockchain.Services.CarbonCreditHandler;
using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;
using CarbonBlockchain.Services.WebSocketHosted.Dtos;

namespace CarbonBlockchain.Services.WebSocketHosted;

public class WebSocketHostedClientService(IConfiguration configuration, IServiceProvider serviceProvider) : BackgroundService, IWebSocketHostedClientService
{
    private readonly TimeSpan _reconnectInterval = TimeSpan.FromSeconds(3);
    private readonly string? _webSocketConnectionUrl = configuration.GetConnectionString("WebSocketConnection");
    private readonly Channel<object> _messageQueue = Channel.CreateUnbounded<object>();

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {

            if (string.IsNullOrEmpty(_webSocketConnectionUrl))
                throw new ArgumentNullException("Error to start connection. Base URL not configured.");

            try
            {
                using var webSocket = new ClientWebSocket();

                await webSocket.ConnectAsync(new Uri(_webSocketConnectionUrl), cancellationToken);
                Console.WriteLine("Connected in web socket successfully.");

                await Task.WhenAll(
                    ReceiveMessagesAsync(webSocket, cancellationToken),
                    ProcessQueueAsync(webSocket, cancellationToken),
                    SendHeartbeatAsync(webSocket, cancellationToken)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in connection: {ex.Message}. Attempting to reconnect in {_reconnectInterval.TotalSeconds} seconds...");
            }

            await Task.Delay(_reconnectInterval, cancellationToken);
        }
    }

    private async Task ProcessQueueAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        while (await _messageQueue.Reader.WaitToReadAsync(cancellationToken))
        {
            while (_messageQueue.Reader.TryRead(out var message))
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    await SendMessageAsync(webSocket, message);
                }
                else
                {
                    Console.WriteLine("Cannot send message, socket is not open.");
                }
            }
        }
    }


    private async Task SendHeartbeatAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            var heartbeatDto = new WebSocketMessageDto(200, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "Heartbeat");

            await SendMessageAsync(webSocket, heartbeatDto);
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }

    private async Task ReceiveMessagesAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", cancellationToken);
            }
            else if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await HandleReceivedMessageAsync(webSocket, message, OnMessage);
            }
        }
    }

    private async Task SendMessageAsync(WebSocket webSocket, object? message)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true
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


    public async Task SendMessageAsync(object message)
    {
        await _messageQueue.Writer.WriteAsync(message);
    }

    private async Task HandleReceivedMessageAsync(WebSocket webSocket, string message, Func<string?, Task> onMessage)
    {
        try
        {
            WebSocketMessageDto? responseDto = null;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true
            };

            var jsonMessage = JsonSerializer.Deserialize<WebSocketMessageDto>(message, options);

            if (jsonMessage.StatusCode == 200 && jsonMessage.Message as string == "Ack")
            {
                Console.WriteLine("Heartbeat received by the server.");
            }

            if (jsonMessage == null || jsonMessage.Message == null)
            {
                responseDto = new WebSocketMessageDto(
                    400,
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    "Message cannot be empty.");
            }

            if (jsonMessage != null && jsonMessage.StatusCode == 200 && jsonMessage.Message != null)
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

    private async Task OnMessage(string? raw)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                Console.WriteLine("Message is Empty.");
                return;
            }

            object? jsonObj = TryDeserialize(raw);

            var provider = serviceProvider.CreateScope().ServiceProvider;

            switch (jsonObj)
            {
                case List<CarbonCreditCertifierDto> dto:
                    var carbonCreditHandlerService = provider.GetService<ICarbonCreditHandlerService>();
                    await carbonCreditHandlerService.HandleCarbonCreditsAsync(dto);
                    break;
                default:
                    Console.WriteLine("Unknown message type.");
                    break;
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error to read web socket message data: {ex.Message}");
        }
    }

    private object? TryDeserialize(string raw)
    {
        var knownTypes = new List<Type>
        {
            typeof(List<CarbonCreditCertifierDto>),
        };

        foreach (var type in knownTypes)
        {
            try
            {
                var serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    IncludeFields = true
                };

                var obj = JsonSerializer.Deserialize(raw, type, serializerOptions);

                if (obj != null) return obj;
            }
            catch
            {
                continue;
            }
        }

        return null;
    }
}