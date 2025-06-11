using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CarbonCertifier.Services.WebSocketHostedServer;
using CarbonCertifier.Services.WebSocketHostedServer.Dtos;

public class WebSocketHostedServerService : BackgroundService, IWebSocketHostedServerService
{
    private WebSocket? _webSocket;

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

    public async Task ConnectAsync(WebSocket webSocket, WebSocketMessageDto? initialMessage, Func<string, Task> onMessage)
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "New connection started.", CancellationToken.None);
        }

        _webSocket = webSocket;

        Console.WriteLine("New web socket connection started.");

        if (initialMessage != null)
        {
            await SendMessageAsync(webSocket, initialMessage);
        }

        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                using var ms = new MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    ms.Write(buffer, 0, result.Count);
                }
                while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed.", CancellationToken.None);
                    Console.WriteLine("Web socket connection closed.");
                    break;
                }
                
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var received = Encoding.UTF8.GetString(ms.ToArray());
                    await HandleReceivedMessageAsync(webSocket, received, onMessage);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during web socket connection: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Web socket connection closed.");
            _webSocket = null;
        }
    }

    public async Task SendWebSocketMessageAsync(object message)
    {
        if (_webSocket?.State == WebSocketState.Open)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var dto = new WebSocketMessageDto(200, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), jsonMessage);
            await SendMessageAsync(_webSocket, dto);
        }
        else
        {
            Console.WriteLine("No active connection to send the message.");
        }
    }

    private async Task SendMessageAsync(WebSocket webSocket, WebSocketMessageDto message)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to send message: {ex.Message}");
        }
    }

    private async Task HandleReceivedMessageAsync(WebSocket webSocket, string message, Func<string, Task> onMessage)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<WebSocketMessageDto>(message);

            var msg = dto.Message;

            WebSocketMessageDto? response;
            
            if (msg is not string msgStr)
            {
                throw new Exception("Invalid message type.");
            }
            
            if (string.IsNullOrWhiteSpace(msgStr))
            {
                response = new WebSocketMessageDto(400, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "Void message.");
                await SendMessageAsync(webSocket, response);
                return;
            }
            
            if (msgStr == "HEARTBEAT")
            {
                Console.WriteLine("Heartbeat received.");
                response = new WebSocketMessageDto(200, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "ACK");
                await SendMessageAsync(webSocket, response);
                return;
            }
            
            
            await onMessage(msgStr);

            response = new WebSocketMessageDto(200, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "Message received successfully.");
            await SendMessageAsync(webSocket, response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to handle message: {ex.Message}");
        }
    }
}
