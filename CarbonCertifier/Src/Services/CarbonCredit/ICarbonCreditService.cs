using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CreditCarbon.Dtos;
using Microsoft.EntityFrameworkCore.Storage;

namespace CarbonCertifier.Services.CarbonCredit;

public interface ICarbonCreditService
{
    Task GenerateCarbonCreditsAsync(CarbonProjectEntity carbonProject, IDbContextTransaction transaction);

    Task<CarbonCreditDto> GetByIdAsync(long id);

    Task<List<CarbonCreditDto>> GetAllAsync();
    
    Task HandleWebSocketMessageUpdateAsync(object message);
}