using CarbonCertifier.Data;
using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CarbonProject.Dtos;
using CarbonCertifier.Entities.CreditCarbon;
using CarbonCertifier.Entities.CreditCarbon.Dtos;
using CarbonCertifier.Entities.CreditCarbon.Enums;
using CarbonCertifier.Services.WebSocketHosted;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CarbonCertifier.Services.CarbonCredit;

public class CarbonCreditService(CarbonCertifierDbContext dbContext, IWebSocketHostedServerService webSocketHostedServerService) : ICarbonCreditService
{
    public async Task GenerateCarbonCreditsAsync(CarbonProjectEntity carbonProject, IDbContextTransaction transaction)
    {
        try
        {
            var random = new Random();
            var createCounter = random.Next(10, 50);
            var currentYear = DateTime.UtcNow.Year;

            var carbonCredits = new List<CarbonCreditEntity>();

            for (var i = 0; i < createCounter; i++)
            {
                var credit = new CarbonCreditEntity
                {
                    CreditCode = Guid.NewGuid().ToString(),
                    VintageYear = currentYear,
                    TonCO2Quantity = Math.Round(random.NextDouble() * (10 - 1) + 1, 2),
                    Owner = carbonProject.Developer,
                    OwnerDocument = Guid.NewGuid().ToString(),
                    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    CarbonProject = carbonProject,
                    Status = CarbonCreditStatus.AVAILABLE
                };

                carbonCredits.Add(credit);
            }

            await dbContext.CarbonCredits.AddRangeAsync(carbonCredits); 

            carbonProject.CarbonCredits = carbonCredits;

            await dbContext.SaveChangesAsync(); 
            
            var message = carbonCredits.Select(e => e.Adapt<CarbonCreditDto>()).ToList();
            await webSocketHostedServerService.SendWebSocketMessageAsync(message);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
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
                    carbonCreditDto.CarbonProject = e.CarbonProject.Adapt<CarbonProjectSimpleDto>();
                    return carbonCreditDto;
                })
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting carbon credits.", ex);

        }
    }

    //TODO: Implementar atualização de carbon credits via websocket
    public async Task HandleWebSocketMessageUpdateAsync(object message)
    {
        try
        {
            var carbonCredits = (List<CarbonCreditDto>) message;
            if (carbonCredits != null && carbonCredits.Count > 0)
            {
                await UpdateCarbonCreditsAsync(carbonCredits);
            }
            else
            {
                throw new Exception("Error to cast message to List<CarbonCreditDto> type");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling websocket message: {ex.Message}");
        }
    }

    private async Task UpdateCarbonCreditsAsync(List<CarbonCreditDto> carbonCredits)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var tasks = carbonCredits.Select(async i =>
            {
                var entity = await dbContext.CarbonCredits
                    .Where(e => e.CreditCode == i.CreditCode)
                    .FirstOrDefaultAsync();

                entity.Adapt(i);

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            });

            await Task.WhenAll(tasks);
            Console.WriteLine("Carbon credits updated successfully!");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating carbon credits: {ex.Message}");
        }
    }
    
}