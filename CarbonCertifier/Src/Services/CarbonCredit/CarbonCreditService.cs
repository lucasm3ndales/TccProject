using System.Text.Json;
using CarbonCertifier.Data;
using CarbonCertifier.Entities.CarbonCredit;
using CarbonCertifier.Entities.CarbonCredit.Dtos;
using CarbonCertifier.Entities.CarbonCredit.Enums;
using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CarbonProject.Dtos;
using CarbonCertifier.Services.WebSocketHostedServer;
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
            var createCounter = random.Next(5, 10);
            var currentYear = DateTime.UtcNow.Year;

            var carbonCredits = new List<CarbonCreditEntity>();

            for (var i = 0; i < createCounter; i++)
            {
                var credit = new CarbonCreditEntity
                {
                    CreditCode = Guid.NewGuid().ToString(),
                    VintageYear = currentYear,
                    TonCO2Quantity = Math.Round(random.NextDouble() * (10 - 1) + 1, 2),
                    OwnerName = carbonProject.Developer,
                    OwnerDocument = GenerateRandomCnpj(),
                    CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    CarbonProject = carbonProject,
                    Status = CarbonCreditStatus.ISSUED
                };

                carbonCredits.Add(credit);
            }

            await dbContext.CarbonCredits.AddRangeAsync(carbonCredits); 

            carbonProject.CarbonCredits = carbonCredits;

            await dbContext.SaveChangesAsync(); 
            
            var message = carbonCredits.Select(e =>
                {
                    var carbonCreditDto = e.Adapt<CarbonCreditDto>();
                    carbonCreditDto.ProjectCode = e.CarbonProject.ProjectCode;
                    return carbonCreditDto;
                })
                .ToList();
            
            await webSocketHostedServerService.SendWebSocketMessageAsync(message);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error setting carbon credits in carbon project.", ex);
        }
    }
    
    private static string GenerateRandomCnpj()
    {
        var random = new Random();
        var cnpj = string.Empty;
        
        for (var i = 0; i < 14; i++)
        {
            cnpj += random.Next(0, 10).ToString();
        }
        
        return cnpj;
    }

    public async Task<List<CarbonCreditSimpleDto>> UpdateCarbonCreditsAsync(string ids, List<CarbonCreditUpdateDto> dtos)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var idsList = ids.Split(',').ToList();
            
            var carbonCredits = await dbContext
                .CarbonCredits
                .Where(e => idsList.Contains(e.Id.ToString()))
                .Include(e => e.CarbonProject)
                .ToListAsync();

            var updates = new List<CarbonCreditEntity>([]);
            for (var i = 0; i < carbonCredits.Count; i++)
            {
                var entity = carbonCredits.ElementAt(i);

                for (var j = 0; j < dtos.Count; j++)
                {
                    var dto = dtos.ElementAt(j);
                    if (entity.CreditCode != dto.CreditCode) 
                        continue;

                    dto.Adapt(entity);
                    entity.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    updates.Add(entity);
                    break;
                }                
            }
            
            dbContext.CarbonCredits.UpdateRange(updates);
            
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            
            var message = carbonCredits.Select(e =>
                {
                    var carbonCreditDto = e.Adapt<CarbonCreditDto>();
                    carbonCreditDto.ProjectCode = e.CarbonProject.ProjectCode;
                    return carbonCreditDto;
                })
                .ToList();
            
            await webSocketHostedServerService.SendWebSocketMessageAsync(message);
            
            return updates.Select(e => e.Adapt<CarbonCreditSimpleDto>()).ToList();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error to update carbon credits.");
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
                    carbonCreditDto.ProjectCode = e.CarbonProject.ProjectCode;
                    return carbonCreditDto;
                })
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting carbon credits.", ex);

        }
    }

    public async Task HandleWebSocketMessageUpdateAsync(object message)
    {
        try
        {
            Console.WriteLine("Updates received from blockchain.");
            var carbonCredits = JsonSerializer.Deserialize<List<CarbonCreditDto>>((string)message);
            if (carbonCredits is { Count: > 0 })
            {
                await UpdateCarbonCreditsFromBlockchainAsync(carbonCredits);
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

    private async Task UpdateCarbonCreditsFromBlockchainAsync(List<CarbonCreditDto> carbonCredits)
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