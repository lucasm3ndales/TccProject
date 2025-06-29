using CarbonBlockchain.Services.BesuClient;
using CarbonBlockchain.Services.BesuEventHostedClient.Adapters;
using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Reactive.Eth.Subscriptions;

namespace CarbonBlockchain.Services.BesuEventHostedClient;

public class BesuEventHostedClientService : BackgroundService, IBesuEventHostedClientService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly string _url;
    private readonly string _contractAddress;
    private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(1);

    public BesuEventHostedClientService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _url = _configuration.GetConnectionString("BesuSocketConnection");
        _contractAddress = _configuration.GetSection("Besu:CarbonCreditTokenContractAddress").Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"[{DateTime.UtcNow:O}] Starting Besu event client.");
            var subscriptionEnded = new TaskCompletionSource<object>();
            StreamingWebSocketClient client = null;
            EthLogsObservableSubscription logsSubscription = null;

            try
            {
                client = new StreamingWebSocketClient(_url);
                logsSubscription = new EthLogsObservableSubscription(client);
                logsSubscription.GetSubscriptionDataResponsesAsObservable()
                    .Subscribe(
                        log => _ = ProcessLogAsync(log),
                        ex => {
                            Console.WriteLine($"[{DateTime.UtcNow:O}] Error in WebSocket subscription: {ex.Message}");
                            subscriptionEnded.TrySetResult(null);
                        },
                        () => {
                            Console.WriteLine($"[{DateTime.UtcNow:O}] WebSocket subscription closed cleanly.");
                            subscriptionEnded.TrySetResult(null);
                        });

                await client.StartAsync();
                Console.WriteLine($"[{DateTime.UtcNow:O}] WebSocket connection with the Besu network established.");

                var filter = Event<CarbonCreditUpdatesEvent>.GetEventABI().CreateFilterInput(_contractAddress);

                await logsSubscription.SubscribeAsync(filter);
                Console.WriteLine($"[{DateTime.UtcNow:O}] Successfully subscribed to contract events: {_contractAddress}. Waiting for events...");

                await subscriptionEnded.Task;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.UtcNow:O}] Failed to start or connect the WebSocket client: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"[{DateTime.UtcNow:O}] Cleaning up previous connection.");
                if (logsSubscription != null)
                {
                    try { await logsSubscription.UnsubscribeAsync(); } catch { }
                }
                if (client != null)
                {
                    try { await client.StopAsync(); } catch { }
                }
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"[{DateTime.UtcNow:O}] Attempting to reconnect in {_reconnectDelay.TotalSeconds} seconds...");
                await Task.Delay(_reconnectDelay, stoppingToken);
            }
        }
    }

    private async Task ProcessLogAsync(FilterLog log)
    {
        try
        {
            var decoded = Event<CarbonCreditUpdatesEvent>.DecodeEvent(log);
            if (decoded?.Event == null) return;

            Console.WriteLine($"[{DateTime.UtcNow:O}] Event received! Function: {decoded.Event.Func} | Operator: {decoded.Event.Operator}");

            var tokenIds = decoded.Event.TokenIds;
            var creditCodes = decoded.Event.CreditCodes;

            if (tokenIds?.Count > 0 && creditCodes?.Count > 0)
            {
                using var scope = _serviceProvider.CreateScope();
                var besuClientService = scope.ServiceProvider.GetRequiredService<IBesuClientService>();
                await besuClientService.HandleCarbonCreditTokensUpdatesAsync(creditCodes);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.UtcNow:O}] Error processing contract event: {ex.Message}");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"[{DateTime.UtcNow:O}] The Besu event service is stopping.");
        await base.StopAsync(cancellationToken);
    }
}