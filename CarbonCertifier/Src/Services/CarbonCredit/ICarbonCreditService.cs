using CarbonCertifier.Entities.CarbonCredit.Dtos;
using CarbonCertifier.Entities.CarbonProject;
using Microsoft.EntityFrameworkCore.Storage;

namespace CarbonCertifier.Services.CarbonCredit;

public interface ICarbonCreditService
{
    Task GenerateCarbonCreditsAsync(CarbonProjectEntity carbonProject, IDbContextTransaction transaction);

    Task<CarbonCreditDto> GetByIdAsync(long id);

    Task<List<CarbonCreditDto>> GetAllAsync();
    
    Task HandleWebSocketMessageUpdateAsync(object message);

    Task<List<CarbonCreditSimpleDto>> UpdateCarbonCreditsAsync(string ids, List<CarbonCreditUpdateDto> dtos);
}