using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using CarbonBlockchain.Services.CarbonCreditHandler;
using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;
using CarbonBlockchain.Services.WebSocketHosted.Dtos;

namespace CarbonBlockchain.Services.WebSocketHosted;

public class WebSocketHostedClientService(IConfiguration configuration, IServiceProvider serviceProvider)
    : BackgroundService, IWebSocketHostedClientService
{
    private readonly TimeSpan _reconnectInterval = TimeSpan.FromSeconds(3);
    private readonly string? _webSocketConnectionUrl = configuration.GetConnectionString("WebSocketConnection");
    private readonly Channel<WebSocketMessageDto> _messageQueue = Channel.CreateUnbounded<WebSocketMessageDto>();

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
                Console.WriteLine(
                    $"Error in connection: {ex.Message}. Attempting to reconnect in {_reconnectInterval.TotalSeconds} seconds...");
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
            var heartbeatDto =
                new WebSocketMessageDto(200, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "Heartbeat");

            await SendMessageAsync(webSocket, heartbeatDto);
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
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
            }
            while (!result.EndOfMessage);

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

            if (jsonMessage.StatusCode == 200 && jsonMessage.Message as string == "Ack")
            {
                Console.WriteLine("Ack returned by the server.");
            }

            if (jsonMessage.StatusCode == 400)
            {
                Console.WriteLine("BadRequest returned by the server.");
            }

            if (jsonMessage != null && jsonMessage.StatusCode == 200 && jsonMessage.Message != null)
            {
                await onMessage(jsonMessage.Message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to handle web socket received message: {ex.Message}");
        }
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