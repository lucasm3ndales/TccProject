using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CreditCarbon.Dtos;

namespace CarbonCertifier.Services.CarbonCredit;

public interface ICarbonCreditService
{
    Task SetCarbonCreditsAsync(CarbonProjectEntity carbonProject);

    Task<CarbonCreditDto> GetByIdAsync(long id);

    Task<List<CarbonCreditDto>> GetAllAsync();
    
    Task HandleWebSocketMessageUpdateAsync(string raw);
}