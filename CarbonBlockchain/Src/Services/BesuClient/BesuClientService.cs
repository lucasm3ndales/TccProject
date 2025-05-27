using CarbonBlockchain.Services.BesuClient.Adapters;
using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;
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

    public async Task<bool> TokenizeCarbonCreditAsync(CarbonCreditTokenDto dto)
    {
        try
        {
            var function = new MintCarbonCreditFunction
            {
                To = _web3.TransactionManager.Account.Address,
                Data = new CarbonCreditMetadata
                {
                    CreditCode = dto.CreditCode,
                    VintageYear = dto.VintageYear,
                    TonCO2Quantity = Web3.Convert.ToWei(dto.TonCO2Quantity),
                    Status = dto.Status,
                    OwnerName = dto.OwnerName,
                    OwnerDocument = dto.OwnerDocument,
                    CreatedAt = dto.CreatedAt,
                    UpdatedAt = dto.UpdatedAt,
                    ProjectCode = dto.ProjectCode,
                    ProjectName = dto.ProjectName,
                    ProjectLocation = dto.ProjectLocation,
                    ProjectDeveloper = dto.ProjectDeveloper,
                    ProjectCreatedAt = dto.ProjectCreatedAt,
                    ProjectUpdatedAt = dto.ProjectUpdatedAt,
                    ProjectType = dto.ProjectType,
                    ProjectStatus = dto.ProjectStatus
                },
                Message = "Tokenização de crédito de carbono"
            };

            var handler = _web3.Eth.GetContractTransactionHandler<MintCarbonCreditFunction>();
            var receipt = await handler.SendRequestAndWaitForReceiptAsync(_carbonCreditTokenContractAddress, function);

            Console.WriteLine($"Transaction Hash: {receipt.TransactionHash}");
            return receipt.Status.Value == 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to tokenize carbon credit {dto.CreditCode}: {ex.Message}");
            throw;
        }
    }

    public Task GetCarbonCreditTokensAsync(List<string> creditCodes)
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to get carbon credit tokens from carbon net blockchain: {ex.Message}");
            throw;
        }
    }
}