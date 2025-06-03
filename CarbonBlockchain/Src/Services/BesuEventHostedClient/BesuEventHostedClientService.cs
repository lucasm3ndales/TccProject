using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Reactive.Eth.Subscriptions;

namespace CarbonBlockchain.Services.BesuEventHostedClient;

public class BesuEventHostedClientService : BackgroundService, IBesuEventHostedClientService
{
    private readonly string _url;
    private StreamingWebSocketClient _client;
    private EthNewBlockHeadersObservableSubscription _observableSubscription;
    private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

    public BesuEventHostedClientService(IConfiguration configuration)
    {
        _url = configuration.GetConnectionString("BesuSocketConnection") ??
               throw new ArgumentNullException("BesuSocketConnection not found in appsettings");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Starting WebSocket connection with Besu network.");
                await StartAsync();
                await SubscribeToBlockHeadersAsync();

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_client.WebSocketState != System.Net.WebSockets.WebSocketState.Open)
                    {
                        Console.WriteLine("WebSocket connection lost. Attempting to reconnect...");
                        break;
                    }

                    await Task.Delay(_interval, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WebSocket connection: {ex.Message}");
            }
            finally
            {
                await StopAsync();
                if (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine($"Reconnecting in {_reconnectDelay.TotalSeconds} seconds...");
                    await Task.Delay(_reconnectDelay, stoppingToken);
                }
            }
        }
    }

    private async Task StartAsync()
    {
        _client = new StreamingWebSocketClient(_url);
        await _client.StartAsync();
    }

    private async Task SubscribeToBlockHeadersAsync()
    {
        _observableSubscription = new EthNewBlockHeadersObservableSubscription(_client);

        _observableSubscription.GetSubscriptionDataResponsesAsObservable()
            .Subscribe(blockHeader =>
            {
                // Console.WriteLine($"New block received: {blockHeader.Number.Value}");
                // Handle net listener here.
            });

        await _observableSubscription.SubscribeAsync();
    }

    private async Task StopAsync()
    {
        if (_observableSubscription != null)
        {
            await _observableSubscription.UnsubscribeAsync();
        }

        if (_client != null)
        {
            await _client.StopAsync();
        }
    }
}