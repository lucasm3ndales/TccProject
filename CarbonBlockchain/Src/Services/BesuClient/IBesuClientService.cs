using CarbonBlockchain.Services.BesuClient.Adapters;

namespace CarbonBlockchain.Services.BesuClient;

public interface IBesuClientService
{
    Task<bool> MintCarbonCreditsInBatchAsync(List<CarbonCreditTokenData> dtos);
    Task<CarbonCreditTokenData?> GetCarbonCreditTokenDataAsync(string tokenId);
    Task<bool> TransferCarbonCreditTokensInBatchAsync(string from, string to, List<string> tokenIds);
}