using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CarbonBlockchain.Services.CarbonCreditHandler;
using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;
using CarbonBlockchain.Services.WebSocketHostedClient.Dtos;

namespace CarbonBlockchain.Services.WebSocketHostedClient;

public class WebSocketHostedClientService(
    IConfiguration configuration,
    IServiceProvider serviceProvider)
    : BackgroundService, IWebSocketHostedClientService
{
    private ClientWebSocket? _webSocket;
    private readonly TimeSpan _reconnectInterval = TimeSpan.FromSeconds(3);
    private readonly string? _serverUrl = configuration.GetConnectionString("WebSocketConnection");

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (string.IsNullOrEmpty(_serverUrl))
            {
                Console.WriteLine("WebSocket connection URL is not configured.");
                return;
            }

            _webSocket = new ClientWebSocket();

            try
            {
                await _webSocket.ConnectAsync(new Uri(_serverUrl), cancellationToken);
                Console.WriteLine("Successfully connected to the WebSocket server.");

                var receiveTask = ReceiveMessagesAsync(_webSocket, cancellationToken);
                var heartbeatTask = SendHeartbeatAsync(_webSocket, cancellationToken);

                await Task.WhenAny(receiveTask, heartbeatTask);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket connection error: {ex.Message}");
            }

            Console.WriteLine($"Reconnecting in {_reconnectInterval.TotalSeconds} seconds...");
            await Task.Delay(_reconnectInterval, cancellationToken);
        }
    }

    private async Task SendHeartbeatAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            var heartbeat = new WebSocketMessageDto(200, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "HEARTBEAT");
            await SendMessageAsync(webSocket, heartbeat, cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
        }
    }

    private async Task ReceiveMessagesAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            try
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
                    Console.WriteLine("Connection closed by the server.");
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client",
                        cancellationToken);
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(ms.ToArray());
                    await HandleReceivedMessageAsync(message, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while receiving WebSocket message: {ex.Message}");
            }
        }
    }

    private async Task HandleReceivedMessageAsync(string message, CancellationToken cancellationToken)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<WebSocketMessageDto>(message);
            
            var msg = dto.Message;
            
            if (msg is not string msgStr)
            {
                throw new Exception("Invalid message type.");
            }
            
            if (string.IsNullOrWhiteSpace(msgStr))
            {
                Console.WriteLine("Received invalid WebSocket message.");
                return;
            }
            
            if (dto.StatusCode == 200 && msgStr == "ACK")
            {
                Console.WriteLine("Heartbeat acknowledged by the server.");
            }
            
            if (dto.StatusCode == 400)
            {
                Console.WriteLine("Bad request received from the server.");
            }
            
            if (dto.StatusCode == 200 && msgStr != "ACK")
            {
                Console.WriteLine("Data message received successfully.");
                await OnMessageAsync(msgStr, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing received WebSocket message: {ex.Message}");
        }
    }

    private async Task OnMessageAsync(string message, CancellationToken cancellationToken)
    {
        try
        {
            var data = JsonSerializer.Deserialize<List<CarbonCreditCertifierDto>>(message);

            if (data != null)
            {
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<ICarbonCreditHandlerService>();
                await handler.HandleCertifiedCarbonCreditsAsync(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling received data: {ex.Message}");
        }
    }

    private async Task SendMessageAsync(
        WebSocket socket, 
        WebSocketMessageDto message,
        CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);
            await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending WebSocket message: {ex.Message}");
        }
    }
    
    public async Task SendWebSocketMessageAsync(object message)
    {
        if (_webSocket is not { State: WebSocketState.Open })
        {
            throw new Exception("WebSocket is not connected.");
        }

        try
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var dto = new WebSocketMessageDto(200, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), jsonMessage);
            await SendMessageAsync(_webSocket, dto, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while sending WebSocket message: {ex.Message}");
        }
    }
}