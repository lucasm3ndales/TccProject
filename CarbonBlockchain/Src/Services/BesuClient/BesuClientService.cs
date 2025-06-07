using CarbonBlockchain.Services.BesuClient.Adapters;
using CarbonBlockchain.Services.BesuClient.Dtos;
using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;
using CarbonBlockchain.Services.CarbonCreditHandler.Enums;
using CarbonBlockchain.Services.WebSocketHostedClient;
using CarbonBlockchain.Services.WebSocketHostedClient.Dtos;
using Nethereum.Contracts;
using Nethereum.Util;
using Nethereum.Web3;

namespace CarbonBlockchain.Services.BesuClient;

public class BesuClientService: IBesuClientService
{
    private readonly string _rpcBaseUrl;
    private readonly string _signerPrivateKey;
    private readonly string _contractAddress;
    private readonly Web3 _web3;
    private readonly IWebSocketHostedClientService _webSocketHostedClientService;


    public BesuClientService(IConfiguration configuration, IWebSocketHostedClientService webSocketHostedClientService)
    {
        _webSocketHostedClientService = webSocketHostedClientService;
        _rpcBaseUrl = configuration.GetConnectionString("BesuHttpConnection");
        _signerPrivateKey = configuration.GetSection("Besu").GetSection("SignerPrivateKey").Value;
        _contractAddress = configuration.GetSection("Besu").GetSection("CarbonCreditTokenContractAddress").Value;
        var account = new Nethereum.Web3.Accounts.Account(_signerPrivateKey);
        _web3 = new Web3(account, _rpcBaseUrl);
    }

    public async Task<bool> MintCarbonCreditsInBatchAsync(List<CarbonCreditTokenStructData> dtos)
    {
        if (dtos == null || dtos.Count == 0)
            throw new ArgumentException("No carbon credits provided.");

        var function = new BatchMintCarbonCreditsFunction()
        {
            To = _web3.TransactionManager.Account.Address,
            Credits = dtos
        };

        var handler = _web3.Eth.GetContractTransactionHandler<BatchMintCarbonCreditsFunction>();

        var receipt = await handler.SendRequestAndWaitForReceiptAsync(_contractAddress, function);

        if (receipt.Status.Value == 1)
        {
            Console.WriteLine($"Transaction Hash: {receipt.TransactionHash}");
            return true;
        }

        Console.WriteLine("Transaction failed or returned false.");
        return false;
    }

    public async Task<CarbonCreditTokenOutData?> GetCarbonCreditTokenDataAsync(string creditCode)
    {
        try
        {
            var function = new GetCarbonCreditFunction()
            {
                CreditCode = creditCode
            };

            var queryHandler = _web3.Eth.GetContractQueryHandler<GetCarbonCreditFunction>();

            var result = await queryHandler.QueryDeserializingToObjectAsync<CarbonCreditTokenOutData>(
                function,
                _contractAddress);

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to get carbon credit: {ex.Message}");
            throw;
        }
    }
    
    // public async Task<bool> TransferCarbonCreditTokensInBatchAsync(TransferCarbonCreditTokensDto dto)
    // {
    //     try
    //     {
    //         var addressUtil = new AddressUtil();
    //
    //         if (!addressUtil.IsValidEthereumAddressHexFormat(dto.To))
    //             throw new ArgumentException($"Invalid 'to' address: {dto.To}");
    //
    //         if (!addressUtil.IsValidEthereumAddressHexFormat(dto.From))
    //             throw new ArgumentException($"Invalid 'from' address: {dto.From}");
    //
    //         if (dto.CreditCodes == null || dto.CreditCodes.Count == 0)
    //             throw new ArgumentException("No carbon credit tokens provided.");
    //
    //         var function = new BatchTransferCarbonCreditsFunction()
    //         {
    //             From = dto.From,
    //             To = dto.To,
    //             CreditCodes = dto.CreditCodes
    //         };
    //
    //         var handler = _web3.Eth.GetContractTransactionHandler<BatchTransferCarbonCreditsFunction>();
    //
    //         var receipt = await handler.SendRequestAndWaitForReceiptAsync(_contractAddress, function);
    //
    //         if (receipt.Status.Value == 1)
    //         {
    //             Console.WriteLine($"Transaction successful. Hash: {receipt.TransactionHash}");
    //             return true;
    //         }
    //
    //         Console.WriteLine($"Transaction failed. Hash: {receipt.TransactionHash}");
    //         return false;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error transferring carbon credits from {dto.From} to {dto.To}: {ex.Message}");
    //         throw;
    //     }
    // }
    //
    //
    // public async Task<bool> RetireCarbonCreditTokensInBatchAsync(List<string> creditCodes)
    // {
    //     try
    //     {
    //         if (creditCodes == null || creditCodes.Count == 0)
    //             throw new ArgumentException("No carbon credit tokens provided.");
    //
    //         var function = new BatchRetireCarbonCreditsFunction()
    //         {
    //             CreditCodes = creditCodes
    //         };
    //
    //         var handler = _web3.Eth.GetContractTransactionHandler<BatchRetireCarbonCreditsFunction>();
    //
    //         var receipt = await handler.SendRequestAndWaitForReceiptAsync(_contractAddress, function);
    //
    //         if (receipt.Status.Value == 1)
    //         {
    //             Console.WriteLine($"Retire transaction successful. Hash: {receipt.TransactionHash}");
    //             return true;
    //         }
    //
    //         Console.WriteLine($"Retire transaction failed. Hash: {receipt.TransactionHash}");
    //         return false;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error retiring carbon credits: {ex.Message}");
    //         throw;
    //     }
    // }
    //
    // public async Task<bool> CancelCarbonCreditTokensInBatchAsync(List<string> creditCodes)
    // {
    //     try
    //     {
    //         if (creditCodes == null || creditCodes.Count == 0)
    //             throw new ArgumentException("No carbon credit tokens provided.");
    //
    //         var function = new BatchCancelCarbonCreditsFunction()
    //         {
    //             CreditCodes = creditCodes
    //         };
    //
    //         var handler = _web3.Eth.GetContractTransactionHandler<BatchCancelCarbonCreditsFunction>();
    //
    //         var receipt = await handler.SendRequestAndWaitForReceiptAsync(_contractAddress, function);
    //
    //         if (receipt.Status.Value == 1)
    //         {
    //             Console.WriteLine($"Cancel transaction successful. Hash: {receipt.TransactionHash}");
    //             return true;
    //         }
    //
    //         Console.WriteLine($"Cancel transaction failed. Hash: {receipt.TransactionHash}");
    //         return false;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error canceling carbon credits: {ex.Message}");
    //         throw;
    //     }
    // }
    //
    // public async Task<bool> AvailableCarbonCreditTokensInBatchAsync(List<string> creditCodes)
    // {
    //     try
    //     {
    //         if (creditCodes == null || creditCodes.Count == 0)
    //             throw new ArgumentException("No carbon credit tokens provided.");
    //
    //         var function = new BatchAvailableCarbonCreditsFunction()
    //         {
    //             CreditCodes = creditCodes
    //         };
    //
    //         var handler = _web3.Eth.GetContractTransactionHandler<BatchAvailableCarbonCreditsFunction>();
    //
    //         var receipt = await handler.SendRequestAndWaitForReceiptAsync(_contractAddress, function);
    //
    //         if (receipt.Status.Value == 1)
    //         {
    //             Console.WriteLine($"Available transaction successful. Hash: {receipt.TransactionHash}");
    //             return true;
    //         }
    //
    //         Console.WriteLine($"Available transaction failed. Hash: {receipt.TransactionHash}");
    //         return false;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error checking availability of carbon credits: {ex.Message}");
    //         throw;
    //     }
    // }
    //
    // public async Task<List<CarbonCreditTokenData>> GetCarbonCreditsInBatchAsync(List<string> creditCodes)
    // {
    //     try
    //     {
    //         if (creditCodes == null || creditCodes.Count == 0)
    //             throw new ArgumentException("No credit codes provided.");
    //
    //         var function = new BatchGetCarbonCreditsFunction()
    //         {
    //             CreditCodes = creditCodes
    //         };
    //
    //         var queryHandler = _web3.Eth.GetContractQueryHandler<BatchGetCarbonCreditsFunction>();
    //
    //         var result = await queryHandler.QueryDeserializingToObjectAsync<CarbonCreditTokenDataList>(
    //             function,
    //             _contractAddress);
    //
    //         return result.CarbonCredits;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error fetching carbon credits batch: {ex.Message}");
    //         throw;
    //     }
    // }
    //
    // public async Task HandleCarbonCreditTokensUpdatesAsync(List<string> creditCodes)
    // {
    //     try
    //     {
    //         var tokens = await GetCarbonCreditsInBatchAsync(creditCodes);
    //         var carbonCredits = new List<CarbonCreditCertifierDto>([]);
    //         
    //         for (var i = 0; i < tokens.Count; i++)
    //         {
    //             var token = tokens[i];
    //             carbonCredits.Add(AdaptToCarbonCreditCertifierDto(token));
    //         }
    //         
    //         var webSocketMessageDto = new WebSocketMessageDto(200, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), carbonCredits);
    //         
    //         _webSocketHostedClientService.SendMessageAsync(webSocketMessageDto);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error handle carbon credits tokens: {ex.Message}");
    //         throw;
    //     }
    // }
    //
    // private CarbonCreditCertifierDto AdaptToCarbonCreditCertifierDto(CarbonCreditTokenData carbonCreditTokenData)
    // {
    //     return new ()
    //     {
    //         CreditCode = carbonCreditTokenData.CreditCode,
    //         VintageYear = (int)carbonCreditTokenData.VintageYear,
    //         TonCO2Quantity = (double)carbonCreditTokenData.TonCO2Quantity,
    //         Status = Enum.Parse<CarbonCreditStatus>(carbonCreditTokenData.Status),
    //         OwnerName = carbonCreditTokenData.OwnerName,
    //         OwnerDocument = carbonCreditTokenData.OwnerDocument,
    //         CreatedAt = (long)carbonCreditTokenData.CreatedAt,
    //         UpdatedAt = (long)carbonCreditTokenData.UpdatedAt,
    //         ProjectCode = carbonCreditTokenData.ProjectCode
    //     };
    // }
}