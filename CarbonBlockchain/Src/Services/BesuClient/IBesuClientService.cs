using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;

namespace CarbonBlockchain.Services.BesuClient;

public interface IBesuClientService
{
    Task<bool> TokenizeCarbonCreditAsync(CarbonCreditTokenDto dto);

    Task GetCarbonCreditTokensAsync(List<string> creditCodes);
}