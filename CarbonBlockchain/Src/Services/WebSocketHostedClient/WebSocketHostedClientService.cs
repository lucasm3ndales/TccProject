using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using CarbonBlockchain.Services.CarbonCreditHandler;
using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;
using CarbonBlockchain.Services.WebSocketHostedClient.Dtos;

namespace CarbonBlockchain.Services.WebSocketHostedClient;

public class WebSocketHostedClientService(IConfiguration configuration, IServiceProvider serviceProvider)
    : BackgroundService, IWebSocketHostedClientService
{
    private readonly TimeSpan _reconnectInterval = TimeSpan.FromSeconds(3);
    private readonly string? _webSocketConnectionUrl = configuration.GetConnectionString("WebSocketConnection");
    private readonly Channel<WebSocketMessageDto> _messageQueue = Channel.CreateUnbounded<WebSocketMessageDto>();
    private ClientWebSocket? _webSocket;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var queueProcessingTask = ProcessQueueAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            if (string.IsNullOrEmpty(_webSocketConnectionUrl))
                throw new ArgumentNullException("Error to start connection. Base URL not configured.");

            _webSocket = new ClientWebSocket();

            try
            {
                await _webSocket.ConnectAsync(new Uri(_webSocketConnectionUrl), cancellationToken);
                Console.WriteLine("Connected in web socket successfully.");

                var receiveTask = ReceiveMessagesAsync(_webSocket, cancellationToken);
                var heartbeatTask = SendHeartbeatAsync(_webSocket, cancellationToken);

                await Task.WhenAny(receiveTask, heartbeatTask);

                if (_webSocket.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection",
                        cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error in connection: {ex.Message}. Attempting to reconnect in {_reconnectInterval.TotalSeconds} seconds...");
            }

            await Task.Delay(_reconnectInterval, cancellationToken);
        }

        _messageQueue.Writer.Complete();
        await queueProcessingTask;
    }

    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        while (await _messageQueue.Reader.WaitToReadAsync(cancellationToken))
        {
            while (_messageQueue.Reader.TryRead(out var message))
            {
                while ((_webSocket == null || _webSocket.State != WebSocketState.Open) &&
                       !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(500, cancellationToken);
                }

                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    await SendMessageAsync(_webSocket!, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message: {ex.Message}");
                }
            }
        }
    }


    private async Task SendHeartbeatAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            var heartbeatDto =
                new WebSocketMessageDto(200, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "Heartbeat");

            await SendMessageAsync(webSocket, heartbeatDto);
            await Task.Delay(TimeSpan.FromMinutes(2), cancellationToken);
        }
    }

    private async Task ReceiveMessagesAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            using var ms = new MemoryStream();
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                ms.Write(buffer, 0, result.Count);
            } while (!result.EndOfMessage);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", cancellationToken);
            }
            else if (result.MessageType == WebSocketMessageType.Text)
            {
                ms.Seek(0, SeekOrigin.Begin);
                var message = Encoding.UTF8.GetString(ms.ToArray());
                await HandleReceivedMessageAsync(webSocket, message, OnMessage);
            }
        }
    }


    private async Task SendMessageAsync(WebSocket webSocket, WebSocketMessageDto? message)
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

            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message through WebSocket: {ex.Message}");
        }
    }


    public async Task SendMessageAsync(WebSocketMessageDto message)
    {
        await _messageQueue.Writer.WriteAsync(message);
    }

    private async Task HandleReceivedMessageAsync(WebSocket webSocket, string message, Func<object?, Task> onMessage)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IncludeFields = true
            };

            var jsonMessage = JsonSerializer.Deserialize<WebSocketMessageDto>(message, options);

            var messageStr = GetMessageAsString(jsonMessage?.Message);

            if (jsonMessage.StatusCode == 200 && !string.IsNullOrEmpty(messageStr) && messageStr == "Ack")
            {
                Console.WriteLine("Ack returned by the server.");
                return;
            }

            if (jsonMessage.StatusCode == 400)
            {
                Console.WriteLine("BadRequest returned by the server.");
                return;
            }

            if (jsonMessage != null && jsonMessage.StatusCode == 200 && jsonMessage.Message != null)
            {
                Console.WriteLine("Message received.");
                await onMessage(jsonMessage.Message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to handle web socket received message: {ex.Message}");
        }
    }

    private string? GetMessageAsString(object? message)
    {
        if (message is null)
            return null;

        if (message is string str)
            return str;

        if (message is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.String)
                return jsonElement.GetString();
        }

        return null;
    }

    private async Task OnMessage(object? message)
    {
        try
        {
            if (message == null)
            {
                throw new Exception("Message is Empty.");
            }

            var jsonMessage = JsonSerializer.Serialize(message);
            var jsonObj = JsonSerializer.Deserialize<List<CarbonCreditCertifierDto>>(jsonMessage);

            if (jsonObj != null)
            {
                var carbonCreditHandlerService = serviceProvider
                    .CreateScope()
                    .ServiceProvider
                    .GetService<ICarbonCreditHandlerService>();

                await carbonCreditHandlerService.HandleCertifiedCarbonCreditsAsync(jsonObj);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to read web socket message data: {ex.Message}");
        }
    }
}