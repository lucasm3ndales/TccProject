using System.Numerics;
using System.Text.Json;
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

public class BesuClientService : IBesuClientService
{
    private readonly string _rpcBaseUrl;
    private readonly string _signerPrivateKey;
    private readonly string _contractAddress;
    private readonly IWebSocketHostedClientService _webSocketHostedClientService;


    public BesuClientService(IConfiguration configuration, IWebSocketHostedClientService webSocketHostedClientService)
    {
        _webSocketHostedClientService = webSocketHostedClientService;
        _rpcBaseUrl = configuration.GetConnectionString("BesuHttpConnection");
        _signerPrivateKey = configuration.GetSection("Besu").GetSection("SignerPrivateKey").Value;
        _contractAddress = configuration.GetSection("Besu").GetSection("CarbonCreditTokenContractAddress").Value;
    }

    public async Task<bool> MintCarbonCreditsInBatchAsync(List<CarbonCreditTokenStructData> dtos)
    {
        if (dtos == null || dtos.Count == 0)
            throw new ArgumentException("No carbon credits provided.");
        
        var account = new Nethereum.Web3.Accounts.Account(_signerPrivateKey);
        var web3 = new Web3(account, _rpcBaseUrl);
        var function = new BatchMintCarbonCreditsFunction()
        {
            To = web3.TransactionManager.Account.Address,
            Credits = dtos
        };

        var handler = web3.Eth.GetContractTransactionHandler<BatchMintCarbonCreditsFunction>();

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
            
            var account = new Nethereum.Web3.Accounts.Account(_signerPrivateKey);
            var web3 = new Web3(account, _rpcBaseUrl);

            var handler = web3.Eth.GetContractQueryHandler<GetCarbonCreditFunction>();

            var result = await handler.QueryDeserializingToObjectAsync<CarbonCreditTokenOutData>(
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

    public async Task<bool> TransferCarbonCreditTokensInBatchAsync(TransferCarbonCreditTokensDto dto)
    {
        try
        {
            var addressUtil = new AddressUtil();

            if (!addressUtil.IsValidEthereumAddressHexFormat(dto.To))
                throw new ArgumentException($"Invalid 'to' address: {dto.To}");

            if (!addressUtil.IsValidEthereumAddressHexFormat(dto.From))
                throw new ArgumentException($"Invalid 'from' address: {dto.From}");

            if (dto.CreditCodes == null || dto.CreditCodes.Count == 0)
                throw new ArgumentException("No carbon credit tokens provided.");

            var function = new BatchTransferCarbonCreditsFunction()
            {
                From = dto.From,
                To = dto.To,
                CreditCodes = dto.CreditCodes,
                OwnerName = dto.ownerName,
                OwnerDocument = dto.ownerDocument
            };
            
            var account = new Nethereum.Web3.Accounts.Account(dto.PrivateKey);
            var web3 = new Web3(account, _rpcBaseUrl);

            var handler = web3.Eth.GetContractTransactionHandler<BatchTransferCarbonCreditsFunction>();

            var receipt = await handler.SendRequestAndWaitForReceiptAsync(_contractAddress, function);

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
            Console.WriteLine($"Error transferring carbon credits from {dto.From} to {dto.To}: {ex.Message}");
            throw;
        }
    }


    public async Task<bool> RetireCarbonCreditTokensInBatchAsync(List<string> creditCodes)
    {
        try
        {
            if (creditCodes == null || creditCodes.Count == 0)
                throw new ArgumentException("No carbon credit tokens provided.");

            var function = new BatchRetireCarbonCreditsFunction()
            {
                CreditCodes = creditCodes
            };
            
            var account = new Nethereum.Web3.Accounts.Account(_signerPrivateKey);
            var web3 = new Web3(account, _rpcBaseUrl);

            var handler = web3.Eth.GetContractTransactionHandler<BatchRetireCarbonCreditsFunction>();

            var receipt = await handler.SendRequestAndWaitForReceiptAsync(_contractAddress, function);

            if (receipt.Status.Value == 1)
            {
                Console.WriteLine($"Retire transaction successful. Hash: {receipt.TransactionHash}");
                return true;
            }

            Console.WriteLine($"Retire transaction failed. Hash: {receipt.TransactionHash}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retiring carbon credits: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> CancelCarbonCreditTokensInBatchAsync(List<string> creditCodes)
    {
        try
        {
            if (creditCodes == null || creditCodes.Count == 0)
                throw new ArgumentException("No carbon credit tokens provided.");

            var function = new BatchCancelCarbonCreditsFunction()
            {
                CreditCodes = creditCodes
            };
            
            var account = new Nethereum.Web3.Accounts.Account(_signerPrivateKey);
            var web3 = new Web3(account, _rpcBaseUrl);

            var handler = web3.Eth.GetContractTransactionHandler<BatchCancelCarbonCreditsFunction>();

            var receipt = await handler.SendRequestAndWaitForReceiptAsync(_contractAddress, function);

            if (receipt.Status.Value == 1)
            {
                Console.WriteLine($"Cancel transaction successful. Hash: {receipt.TransactionHash}");
                return true;
            }

            Console.WriteLine($"Cancel transaction failed. Hash: {receipt.TransactionHash}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error canceling carbon credits: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> AvailableCarbonCreditTokensInBatchAsync(List<string> creditCodes)
    {
        try
        {
            if (creditCodes == null || creditCodes.Count == 0)
                throw new ArgumentException("No carbon credit tokens provided.");

            var function = new BatchAvailableCarbonCreditsFunction()
            {
                CreditCodes = creditCodes
            };

            var account = new Nethereum.Web3.Accounts.Account(_signerPrivateKey);
            var web3 = new Web3(account, _rpcBaseUrl);
            
            var handler = web3.Eth.GetContractTransactionHandler<BatchAvailableCarbonCreditsFunction>();

            var receipt = await handler.SendRequestAndWaitForReceiptAsync(_contractAddress, function);

            if (receipt.Status.Value == 1)
            {
                Console.WriteLine($"Available transaction successful. Hash: {receipt.TransactionHash}");
                return true;
            }

            Console.WriteLine($"Available transaction failed. Hash: {receipt.TransactionHash}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking availability of carbon credits: {ex.Message}");
            throw;
        }
    }

    public async Task<BigInteger> GetBalanceOfAsync(string accountAddress, string privateKey, string creditCode)
    {
        try
        {
            var balanceOfFunction = new BalanceOfFunction()
            {
                Account = accountAddress,
                CreditCode = creditCode
            };
            
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            var web3 = new Web3(account, _rpcBaseUrl);

            var handler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();

            var balance = await handler.QueryAsync<BigInteger>(_contractAddress, balanceOfFunction);

            Console.WriteLine($"Balance: {balance}");

            return balance;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to get balance of account {accountAddress}");
            throw;
        }
    }

private async Task<List<CarbonCreditTokenOutData>> GetCarbonCreditsInBatchAsync(List<string> creditCodes)
    {
        try
        {
            if (creditCodes == null || creditCodes.Count == 0)
                throw new ArgumentException("No credit codes provided.");
    
            var function = new BatchGetCarbonCreditsFunction()
            {
                CreditCodes = creditCodes
            };
    
            var account = new Nethereum.Web3.Accounts.Account(_signerPrivateKey);
            var web3 = new Web3(account, _rpcBaseUrl);
            
            var queryHandler = web3.Eth.GetContractQueryHandler<BatchGetCarbonCreditsFunction>();
    
            var result = await queryHandler.QueryDeserializingToObjectAsync<CarbonCreditTokenListOutData>(
                function,
                _contractAddress);
            
            var tokens = new List<CarbonCreditTokenOutData>();

            for (var i = 0; i < creditCodes.Count; i++)
            {
                var token = new CarbonCreditTokenOutData
                {
                    CreditCode = result.CreditCodes[i],
                    VintageYear = result.VintageYears[i],
                    TonCO2Quantity = result.TonCO2Quantities[i],
                    Status = result.Statuses[i],
                    OwnerName = result.OwnerNames[i],
                    OwnerDocument = result.OwnerDocuments[i],
                    CreatedAt = result.CreatedAts[i],
                    UpdatedAt = result.UpdatedAts[i],
                    ProjectCode = result.ProjectCodes[i],
                };
                tokens.Add(token);
            }
    
            return tokens;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching carbon credits batch: {ex.Message}");
            throw;
        }
    }
    
    public async Task HandleCarbonCreditTokensUpdatesAsync(List<string> creditCodes)
    {
        try
        {
            var tokens = await GetCarbonCreditsInBatchAsync(creditCodes);
            var carbonCredits = new List<CarbonCreditCertifierDto>([]);
            
            for (var i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                carbonCredits.Add(AdaptToCarbonCreditCertifierDto(token));
            }
            
            _webSocketHostedClientService.SendWebSocketMessageAsync(carbonCredits);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handle carbon credits tokens: {ex.Message}");
            throw;
        }
    }
    
    private CarbonCreditCertifierDto AdaptToCarbonCreditCertifierDto(CarbonCreditTokenOutData token)
    {
        return new ()
        {
            CreditCode = token.CreditCode,
            VintageYear = (int)token.VintageYear,
            TonCO2Quantity = token.TonCO2Quantity / 100,
            Status = Enum.Parse<CarbonCreditStatus>(token.Status),
            OwnerName = token.OwnerName,
            OwnerDocument = token.OwnerDocument,
            CreatedAt = (long)token.CreatedAt,
            UpdatedAt = (long)token.UpdatedAt,
            ProjectCode = token.ProjectCode
        };
    }
    
    public async Task<string> SetApprovalForAllAsync(SetApprovalDto dto)
    {
        try
        {
            var setApprovalFunction = new SetApprovalForAllFunction()
            {
                Operator = dto.AccountAddress,
                Approved = dto.IsApproved
            };

            var account = new Nethereum.Web3.Accounts.Account(dto.PrivateKey);
            var web3 = new Web3(account, _rpcBaseUrl);
            
            var handler = web3.Eth.GetContractTransactionHandler<SetApprovalForAllFunction>();

            var receipt = await handler.SendRequestAndWaitForReceiptAsync(_contractAddress, setApprovalFunction);

            Console.WriteLine($"SetApprovalForAll transaction hash: {receipt.TransactionHash}");

            return receipt.TransactionHash;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting approval for operator {dto.AccountAddress}: {ex.Message}");
            throw;
        }
    }
    
    public async Task<bool> IsApprovedForAllAsync(string accountAddress, string privateKey, string operatorAddress)
    {
        try
        {
            var isApprovedFunction = new IsApprovedForAllFunction()
            {
                Account = accountAddress,
                Operator = operatorAddress
            };
            
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            var web3 = new Web3(account, _rpcBaseUrl);

            var handler = web3.Eth.GetContractQueryHandler<IsApprovedForAllFunction>();

            var isApproved = await handler.QueryAsync<bool>(_contractAddress, isApprovedFunction);

            Console.WriteLine($"Is operator {operatorAddress} approved for account {accountAddress}: {isApproved}");

            return isApproved;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking approval for operator {operatorAddress} on account {accountAddress}: {ex.Message}");
            throw;
        }
    }
}