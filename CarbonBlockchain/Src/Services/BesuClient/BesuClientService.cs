using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarbonBlockchain.Services.BesuClient;

public class BesuClientService : IBesuClientService
{
    private readonly string _webSocketUrl;
    private readonly Web3 _web3;
    private WebSocketClient _webSocketClient;

    public BesuClientService(IConfiguration configuration)
    {
        _webSocketUrl = configuration.GetConnectionString("BesuSocketConnection") ??
        throw new ArgumentNullException("BesuSocketConnection not found in appsettings");
        _webSocketClient = new WebSocketClient(_webSocketUrl);
        _web3 = new Web3(_webSocketClient);
    }

}



