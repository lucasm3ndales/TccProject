using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;

namespace CarbonBlockchain.Services.CarbonCreditHandler;

public interface ICarbonCreditHandlerService
{
    Task HandleCarbonCreditsAsync(List<CarbonCreditCertifierDto> dto);
}
