using System.Numerics;
using CarbonBlockchain.Services.BesuClient;
using CarbonBlockchain.Services.CarbonCreditHandler;
using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;
using Mapster;

namespace CarbonBlockchain.Services;

public class CarbonCreditHandlerService(IBesuClientService besuClientService): ICarbonCreditHandlerService
{
    public async Task HandleCertifiedCarbonCreditsAsync(List<CarbonCreditCertifierDto> dtos)
    {
        try
        {
            for (var i = 0; i < dtos.Count; i++)
            {
                var carbonCredit = dtos.ElementAt(i);

                var tokenDto = AdaptToTokenDto(carbonCredit);
                
                var response = await besuClientService.TokenizeCarbonCreditAsync(tokenDto);

                if (!response) throw new Exception("Error to tokenize carbon credits.");
            }
            
            Console.WriteLine("Carbon credits tokenized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to handle carbon credits: {ex.Message}");
        }
    }

    private CarbonCreditTokenDto AdaptToTokenDto(CarbonCreditCertifierDto dto)
    {
        return new CarbonCreditTokenDto
        {
            CreditCode = dto.CreditCode,
            VintageYear = dto.VintageYear,
            TonCO2Quantity = dto.TonCO2Quantity,
            Status = dto.Status.ToString(),
            OwnerName = dto.OwnerName,
            OwnerDocument = dto.OwnerDocument,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            ProjectCode = dto.CarbonProject.ProjectCode,
            ProjectName = dto.CarbonProject.Name,
            ProjectLocation = dto.CarbonProject.Location,
            ProjectDeveloper = dto.CarbonProject.Developer,
            ProjectCreatedAt = dto.CarbonProject.CreatedAt,
            ProjectUpdatedAt = dto.CarbonProject.UpdatedAt,
            ProjectType = dto.CarbonProject.Type.ToString(),
            ProjectStatus = dto.CarbonProject.Status.ToString()
        };
    }
}
