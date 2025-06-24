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
            Console.WriteLine("Tokenization process start.");
            var tokenDtoList = new List<CarbonCreditTokenStructData>([]);
            
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

    private CarbonCreditTokenStructData AdaptToTokenDto(CarbonCreditCertifierDto dto)
    {
        
        
        return new CarbonCreditTokenStructData()
        {
            CreditCode = dto.CreditCode,
            VintageYear = (uint)dto.VintageYear,
            TonCO2Quantity = (uint)Math.Round(dto.TonCO2Quantity * 100),
            Status = dto.Status.ToString(),
            OwnerName = dto.OwnerName,
            OwnerDocument = dto.OwnerDocument,
            CreatedAt = (uint)dto.CreatedAt,
            UpdatedAt = (uint)dto.UpdatedAt,
            ProjectCode = dto.ProjectCode,
        };
    }
}
