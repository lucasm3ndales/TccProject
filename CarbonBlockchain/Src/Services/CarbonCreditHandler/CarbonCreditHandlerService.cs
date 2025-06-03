using System.Numerics;
using CarbonBlockchain.Services.BesuClient;
using CarbonBlockchain.Services.BesuClient.Adapters;
using CarbonBlockchain.Services.CarbonCreditHandler.Dtos;

namespace CarbonBlockchain.Services.CarbonCreditHandler;

public class CarbonCreditHandlerService(IBesuClientService besuClientService): ICarbonCreditHandlerService
{
    public async Task HandleCertifiedCarbonCreditsAsync(List<CarbonCreditCertifierDto> dtos)
    {
        try
        {
            var tokenDtoList = new List<CarbonCreditTokenData>([]);
            
            for (var i = 0; i < dtos.Count; i++)
            {
                var carbonCredit = dtos.ElementAt(i);
                tokenDtoList.Add(AdaptToTokenDto(carbonCredit));
            }
            
            var response = await besuClientService.MintCarbonCreditsInBatchAsync(tokenDtoList);
            
            if(!response) 
                throw new Exception("Error to tokenize carbon credits.");
            
            Console.WriteLine("Carbon credits tokenized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error to handle carbon credits: {ex.Message}");
        }
    }

    private CarbonCreditTokenData AdaptToTokenDto(CarbonCreditCertifierDto dto)
    {
        return new CarbonCreditTokenData
        {
            CreditCode = dto.CreditCode,
            VintageYear = dto.VintageYear,
            TonCO2Quantity = new BigInteger(dto.TonCO2Quantity * 1e18),
            Status = dto.Status,
            OwnerName = dto.OwnerName,
            OwnerDocument = dto.OwnerDocument,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            ProjectCode = dto.ProjectCode,
        };
    }
}
