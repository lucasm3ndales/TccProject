
using CarbonBlockchain.Services.CarbonCreditHandler;
using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;

namespace CarbonBlockchain.Services;

public class CarbonCreditHandlerService : ICarbonCreditHandlerService
{
    public async Task HandleCarbonCreditsAsync(List<CarbonCreditCertifierDto> dto)
    {
        try
        {
            var carbonCredits = dto;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to handle carbon credits: {ex.Message}");
        }
    }
}
