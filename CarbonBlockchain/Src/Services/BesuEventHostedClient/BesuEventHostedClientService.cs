using CarbonBlockchain.Services.BesuClient;
using CarbonBlockchain.Services.BesuEventHostedClient.Adapters;
using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Reactive.Eth.Subscriptions;

namespace CarbonBlockchain.Services.BesuEventHostedClient;

public class BesuEventHostedClientService : BackgroundService, IBesuEventHostedClientService
{
    private readonly string _url;
    private StreamingWebSocketClient _client;
    private EthLogsObservableSubscription _logsSubscription;
    private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private readonly string _contractAddress;
    private readonly IServiceProvider _serviceProvider;
    private bool _isStopping = false;

    public BesuEventHostedClientService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _url = configuration.GetConnectionString("BesuSocketConnection");
        _contractAddress = configuration
            .GetSection("Besu")
            .GetSection("CarbonCreditTokenContractAddress").Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Starting WebSocket connection with Besu network.");
                await StartAsync();
                await SubscribeToContractEventsAsync();

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (_client.WebSocketState != System.Net.WebSockets.WebSocketState.Open)
                    {
                        Console.WriteLine("WebSocket connection lost. Attempting to reconnect...");
                        await StopAsync();
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
                if (!_isStopping)
                {
                    await StopAsync();
                }

                if (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine($"Reconnecting in {_reconnectDelay.TotalSeconds} seconds...");
                    await Task.Delay(_reconnectDelay, stoppingToken);
                }
            }
        }
    }

    private async Task SubscribeToContractEventsAsync()
    {
        _logsSubscription = new EthLogsObservableSubscription(_client);

        var filter = Event<CarbonCreditUpdatesEvent>.GetEventABI().CreateFilterInput(_contractAddress);

        _logsSubscription.GetSubscriptionDataResponsesAsObservable()
            .Subscribe(async (log) =>
            {
                _ = ProcessLogAsync(log);
            });

        await _logsSubscription.SubscribeAsync(filter);
    }

    private async Task ProcessLogAsync(FilterLog log)
    {
        try
        {
            var decoded = Event<CarbonCreditUpdatesEvent>.DecodeEvent(log);
            if (decoded != null)
            {
                Console.WriteLine($"Updates from carbon network. Called Function: {decoded.Event.Func} - Operator: {decoded.Event.Operator}");
                if (decoded.Event.TokenIds != null &&
                    decoded.Event.CreditCodes != null &&
                    decoded.Event.TokenIds.Count != 0 &&
                    decoded.Event.CreditCodes.Count != 0)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var besuClientService = scope.ServiceProvider.GetRequiredService<IBesuClientService>();
                    await besuClientService.HandleCarbonCreditTokensUpdatesAsync(decoded.Event.CreditCodes);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao decodificar evento: {ex.Message}");
        }
    }

    private async Task StartAsync()
    {
        _client = new StreamingWebSocketClient(_url);
        await _client.StartAsync();
    }

    private async Task StopAsync()
    {
        if (_isStopping) return;
        _isStopping = true;

        try
        {
            if (_logsSubscription != null)
            {
                try
                {
                    await _logsSubscription.UnsubscribeAsync();
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("Invalid state to unsubscribe"))
                    {
                        Console.WriteLine($"Error during unsubscribe: {ex.Message}");
                    }
                }
                _logsSubscription = null;
            }

            if (_client != null)
            {
                if (_client.WebSocketState == System.Net.WebSockets.WebSocketState.Open ||
                    _client.WebSocketState == System.Net.WebSockets.WebSocketState.CloseReceived)
                {
                    await _client.StopAsync();
                }
                _client = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during StopAsync: {ex.Message}");
        }
        finally
        {
            _isStopping = false;
        }
    }
}