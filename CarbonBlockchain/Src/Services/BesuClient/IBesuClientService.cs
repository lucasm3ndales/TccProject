using System.Numerics;
using CarbonBlockchain.Services.BesuClient.Adapters;
using CarbonBlockchain.Services.BesuClient.Dtos;

namespace CarbonBlockchain.Services.BesuClient;

public interface IBesuClientService
{
    Task<bool> MintCarbonCreditsInBatchAsync(List<CarbonCreditTokenStructData> dtos);
    Task<CarbonCreditTokenOutData?> GetCarbonCreditTokenDataAsync(string tokenId);
    Task<bool> TransferCarbonCreditTokensInBatchAsync(TransferCarbonCreditTokensDto dto);
    Task<bool> RetireCarbonCreditTokensInBatchAsync(List<string> creditCodes);
    Task<bool> CancelCarbonCreditTokensInBatchAsync(List<string> creditCodes);
    Task<bool> AvailableCarbonCreditTokensInBatchAsync(List<string> creditCodes);
    Task HandleCarbonCreditTokensUpdatesAsync(List<string> creditCodes);
    Task<BigInteger> GetBalanceOfAsync(string accountAddress, string privateKey, string creditCode);
    Task<string> SetApprovalForAllAsync(string operatorAddress, string privateKey, bool approved);
    Task<bool> IsApprovedForAllAsync(string accountAddress, string privateKey, string operatorAddress);
}