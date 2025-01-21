using System.Text.Json;
using CarbonCertifier.Data;
using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CarbonProject.Dtos;
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
        
            var createCounter = random.Next(10, 50);
            
            var currentYear = DateTime.UtcNow.Year;
            
            var tasks = new List<Task<CarbonCreditEntity>>();

            for (var i = 0; i < createCounter; i++)
            {
                var task = Task.Run(() => new CarbonCreditEntity
                {
                    VintageYear = currentYear,
                    TonCO2Quantity = Math.Round(random.NextDouble() * (10 - 1) + 1, 2),
                    PricePerTon = Math.Round(random.NextDouble() * (50 - 10) + 10, 2),
                    Owner = carbonProject.Developer,
                    IsRetired = false,
                    CarbonProject = carbonProject
                });

                tasks.Add(task);
            }
            
            
            var carbonCredits = await Task.WhenAll(tasks);

            await dbContext.CarbonCredits.AddRangeAsync(carbonCredits);

            carbonProject.CarbonCredits = carbonCredits.ToList();
        
            await dbContext.SaveChangesAsync();
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
                .Select(e =>
                {
                    var carbonCreditDto = e.Adapt<CarbonCreditDto>();
                    
                    carbonCreditDto.CarbonProject = e.CarbonProject.Adapt<CarbonProjectDto>(); 
                    
                    return carbonCreditDto;
                })
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting carbon credits.", ex);

        }
    }
    
    public async Task HandleWebSocketMessageUpdateAsync(string raw)
    {
        try
        {
            var carbonCredits = JsonSerializer.Deserialize<List<CarbonCreditDto>>(raw);
            if (carbonCredits != null && carbonCredits.Count > 0)
            {
                await UpdateCarbonCreditsAsync(carbonCredits);
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling websocket message: {ex.Message}");
        }
    }

    public async Task UpdateCarbonCreditsAsync(List<CarbonCreditDto> carbonCredits)
    {
        try
        {
            var tasks = carbonCredits.Select(async i =>
            {
                var entity = await dbContext.CarbonCredits.Where(e => e.CreditCode == i.CreditCode).FirstOrDefaultAsync();

                entity.Adapt(i);
                
                await dbContext.SaveChangesAsync();
            });

            await Task.WhenAll(tasks);
            Console.WriteLine("Carbon credits updated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating carbon credits: {ex.Message}");
        }
    }
    
}