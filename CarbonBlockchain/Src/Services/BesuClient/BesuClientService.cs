using System.Numerics;
using CarbonBlockchain.Services.BesuClient.Adapters;
using CarbonBlockchain.Services.BesuClient.Utils;
using Nethereum.Util;
using Nethereum.Web3;

namespace CarbonBlockchain.Services.BesuClient;

public class BesuClientService : IBesuClientService
{
    private readonly string _rpcBaseUrl;
    private readonly string _signerPrivateKey;
    private readonly string _carbonCreditTokenContractAddress;
    private readonly Web3 _web3;


    public BesuClientService(IConfiguration configuration)
    {
        _rpcBaseUrl = configuration.GetConnectionString("BesuHttpConnection");
        _signerPrivateKey = configuration.GetSection("Besu").GetSection("SignerPrivateKey").Value;
        _carbonCreditTokenContractAddress =
            configuration.GetSection("Besu").GetSection("CarbonCreditTokenContractAddress").Value;

        var account = new Nethereum.Web3.Accounts.Account(_signerPrivateKey);
        _web3 = new Web3(account, _rpcBaseUrl);
    }

    public async Task<bool> MintCarbonCreditsInBatchAsync(List<CarbonCreditTokenData> dtos)
    {
        try
        {
            if (dtos == null || dtos.Count == 0)
            {
                throw new ArgumentException("No carbon credits provided.");
            }

            var function = new BatchMintCarbonCreditsFunction()
            {
                To = _web3.TransactionManager.Account.Address,
                Credits = dtos
            };

            var handler = _web3.Eth.GetContractTransactionHandler<BatchMintCarbonCreditsFunction>();

            var receipt = await handler.SendRequestAndWaitForReceiptAsync(_carbonCreditTokenContractAddress, function);

            if (receipt.Status.Value == 1)
            {
                Console.WriteLine($"Transaction Hash: {receipt.TransactionHash}");
                return true;
            }
            
            Console.WriteLine($"Error to tokenize carbon credits: Transaction failed.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to tokenize carbon credits: {ex.Message}");
            throw;
        }
    }

    public async Task<CarbonCreditTokenData?> GetCarbonCreditTokenDataAsync(string tokenId)
    {
        try
        {
            var function = new GetCarbonCreditFunction()
            {
                CreditCode = tokenId,
            };

            var queryHandler = _web3.Eth.GetContractQueryHandler<GetCarbonCreditFunction>();

            var result = await queryHandler.QueryDeserializingToObjectAsync<CarbonCreditTokenData>(
                function,
                _carbonCreditTokenContractAddress);

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to get carbon credit: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> TransferCarbonCreditTokensInBatchAsync(string to, string from, List<string> creditCodes)
    {
        try
        {
            var addressUtil = new AddressUtil();

            if (!addressUtil.IsValidEthereumAddressHexFormat(to))
                throw new ArgumentException($"Invalid 'to' address: {to}");

            if (!addressUtil.IsValidEthereumAddressHexFormat(from))
                throw new ArgumentException($"Invalid 'from' address: {from}");

            if (creditCodes == null || creditCodes.Count == 0)
                throw new ArgumentException("No carbon credit tokens provided.");

            var function = new BatchTransferCarbonCreditsFunction()
            {
                From = from,
                To = to,
                CreditCodes = creditCodes
            };

            var handler = _web3.Eth.GetContractTransactionHandler<BatchTransferCarbonCreditsFunction>();

            var receipt = await handler.SendRequestAndWaitForReceiptAsync(_carbonCreditTokenContractAddress, function);

            if (receipt.Status.Value == 1)
            {
                Console.WriteLine($"Transaction successful. Hash: {receipt.TransactionHash}");
                return true;
            }
          
            Console.WriteLine($"Transaction failed. Hash: {receipt.TransactionHash}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error transferring carbon credits from {from} to {to}: {ex.Message}");
            throw;
        }
    }
}