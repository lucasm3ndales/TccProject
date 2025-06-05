using CarbonBlockchain.Services.BesuClient.Adapters;
using CarbonBlockchain.Services.BesuClient.Dtos;

namespace CarbonBlockchain.Services.BesuClient;

public interface IBesuClientService
{
    Task<bool> MintCarbonCreditsInBatchAsync(List<CarbonCreditTokenData> dtos);
    // Task<CarbonCreditTokenData?> GetCarbonCreditTokenDataAsync(string tokenId);
    // Task<bool> TransferCarbonCreditTokensInBatchAsync(TransferCarbonCreditTokensDto dto);
    // Task<bool> RetireCarbonCreditTokensInBatchAsync(List<string> creditCodes);
    // Task<bool> CancelCarbonCreditTokensInBatchAsync(List<string> creditCodes);
    // Task<bool> AvailableCarbonCreditTokensInBatchAsync(List<string> creditCodes);
    // Task<List<CarbonCreditTokenData>> GetCarbonCreditsInBatchAsync(List<string> creditCodes);
    // Task HandleCarbonCreditTokensUpdatesAsync(List<string> creditCodes);
}