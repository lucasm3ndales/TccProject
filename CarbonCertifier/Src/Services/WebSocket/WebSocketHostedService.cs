using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CarbonCertifier.Services.CarbonCredit;

namespace CarbonCertifier.Services.Wss;

public class WebSocketHostedService(IServiceProvider provider) : BackgroundService
{
    private Timer Timer { get; set; }
    private List<WebSocket> ConnectedClients { get; set; } = [];

    private readonly ICarbonCreditService _carbonCreditService =
        provider.CreateScope().ServiceProvider.GetRequiredService<ICarbonCreditService>();

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        return Task.CompletedTask;
    }

    private async void DoWorkAsync(object? state)
    {
        try
        {
            var carbonCredits = await _carbonCreditService.GetAllAsync();

            if (carbonCredits.Any())
            {
                var json = JsonSerializer.Serialize(carbonCredits);

                SendToClientsAsync(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to execute ws service: {ex.Message}");
        }
    }

    private async void SendToClientsAsync(string json)
    {
        try
        {
            var clientsToRemove = new List<WebSocket>([]);

            foreach (var c in ConnectedClients)
            {
                if (c.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(json);
                    await c.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    clientsToRemove.Add(c);
                }
            }

            RemoveClients(clientsToRemove);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to send updates to clients: {ex.Message}");
        }
    }

    private void RemoveClients(List<WebSocket> clientsToRemove)
    {
        foreach (var c in clientsToRemove)
        {
            ConnectedClients.Remove(c);
        }
    }

    public async Task HandleWebSocketConnectionAsync(HttpContext context)
    {
        try
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var ws = await context.WebSockets.AcceptWebSocketAsync();
                ConnectedClients.Add(ws);

                await WaitForDisconnectionAsync(ws);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to handle ws connection: {ex.Message}");
        }
    }

    private async Task WaitForDisconnectionAsync(WebSocket ws)
    {
        try
        {
            var buffer = new byte[1024 * 4];

            while (ws.State == WebSocketState.Open)
            {
                await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            ConnectedClients.Remove(ws);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to handle ws disconnection: {ex.Message}");
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await Timer.DisposeAsync();

        foreach (var c in ConnectedClients)
        {
            await c.CloseAsync(WebSocketCloseStatus.NormalClosure, "Service stopping", stoppingToken);
        }

        ConnectedClients.Clear();

        await base.StopAsync(stoppingToken);
    }
}