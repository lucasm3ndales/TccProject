using CarbonCertifier.Data;
using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CreditCarbon;
using CarbonCertifier.Entities.CreditCarbon.Dtos;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CarbonCertifier.Services.CarbonCredit;

public class CarbonCreditService(CarbonCertifierDbContext dbContext) : ICarbonCreditService
{
    public async Task SetCarbonCreditsAsync(CarbonProjectEntity carbonProject)
    {
        try
        {
            var random = new Random();
        
            var createCounter = random.Next(25, 121);
            
            var carbonCredits = Enumerable
                .Range(1, createCounter)
                .Select(_ => new CarbonCreditEntity
                {
                    VintageYear = DateTime.Now.Year, 
                    TonCO2Quantity = Math.Round(random.NextDouble() * (10 - 1) + 1, 2), // Quantidade entre 1 e 10 toneladas
                    PricePerTon = Math.Round(random.NextDouble() * (50 - 10) + 10, 2),  // Preço entre 10 e 50 por tonelada
                    Owner = carbonProject.Developer, 
                    IsRetired = false,
                    CarbonProject = carbonProject 
                })
                .ToList();
            
            carbonProject.CarbonCredits = carbonCredits;
        }
        catch (Exception ex)
        {
           throw new Exception("Error setting carbon credits in carbon project.", ex);
        }
    }

    public async Task<CarbonCreditDto> GetByIdAsync(long id)
    {
        try
        {
            var dbResult = await dbContext.CarbonCredits
                .Include(e => e.CarbonProject)
                .FirstOrDefaultAsync(e => e.Id == id);

            return dbResult.Adapt<CarbonCreditDto>();
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting carbon credit.", ex);
        }
    }

    public async Task<List<CarbonCreditDto>> GetAllAsync()
    {
        try
        {
            var dbResult = await dbContext.CarbonCredits
                .Include(e => e.CarbonProject)
                .ToListAsync();
            
            return dbResult
                .Select(e => e.Adapt<CarbonCreditDto>())
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting carbon credits.", ex);

        }
    }
    
}