using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;

namespace CarbonBlockchain.Services.CarbonCreditHandler;

public interface ICarbonCreditHandlerService
{
    Task HandleCertifiedCarbonCreditsAsync(List<CarbonCreditCertifierDto> dtos);
}
