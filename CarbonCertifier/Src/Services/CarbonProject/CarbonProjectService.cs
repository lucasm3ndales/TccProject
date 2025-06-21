using CarbonCertifier.Data;
using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CarbonProject.Dtos;
using CarbonCertifier.Services.CarbonCredit;
using CarbonCertifier.Services.WebSocketHostedServer;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CarbonCertifier.Services.CarbonProject;

public class CarbonProjectService(
    CarbonCertifierDbContext dbContext, 
    ICarbonCreditService carbonCreditService, 
    IWebSocketHostedServerService webSocketHostedServerService) : ICarbonProjectService
{
    public async Task<CarbonProjectDto> CreateAsync(CarbonProjectCreateDto dto)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var carbonProject = dto.Adapt<CarbonProjectEntity>();
            carbonProject.ProjectCode = Guid.NewGuid().ToString();
            carbonProject.CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            carbonProject.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            var dbResult = await dbContext.CarbonProjects.AddAsync(carbonProject);
            
            await dbContext.SaveChangesAsync();
            
             var message = await carbonCreditService.GenerateCarbonCreditsAsync(dbResult.Entity, transaction);
            
            await transaction.CommitAsync();
            
            await webSocketHostedServerService.SendWebSocketMessageAsync(message);
            
            return carbonProject.Adapt<CarbonProjectDto>();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating carbon project: {ex.Message}");
            throw new Exception("Error creating carbon project.", ex);
        }
    }

    public async Task<CarbonProjectDto> UpdateAsync(long id, CarbonProjectUpdateDto dto)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var dbResult = await dbContext.CarbonProjects.FindAsync(id);

            if (dbResult == null) throw new NullReferenceException("Carbon project not found.");

            dbResult = dto.Adapt<CarbonProjectEntity>();
            dbResult.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return dbResult.Adapt<CarbonProjectDto>();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating carbon project: {ex.Message}");
            throw new Exception("Error updating carbon project.", ex);
        }
    }

    public async Task DeleteAsync(long id)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var dbResult = await dbContext.CarbonProjects.FindAsync(id);

            if (dbResult == null) throw new NullReferenceException("Carbon project not found.");
            
            dbContext.CarbonProjects.Remove(dbResult);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error deleting carbon project: {ex.Message}");
            throw new Exception("Error deleting carbon project.", ex);
        }
    }

    public async Task<CarbonProjectDto> GetByIdAsync(long id)
    {
        try
        {
            var dbResult = await dbContext.CarbonProjects.FindAsync(id);

            if (dbResult == null) throw new NullReferenceException("Carbon project not found.");

            return dbResult.Adapt<CarbonProjectDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting carbon project: {ex.Message}");
            throw new Exception("Error getting carbon project.", ex);
        }
    }

    public async Task<List<CarbonProjectDto>> GetAllAsync()
    {
        try
        {
            var dbResult = await dbContext.CarbonProjects.ToListAsync();

            if (dbResult == null) throw new NullReferenceException("Carbon project not found.");

            return dbResult
                .Select(e => e.Adapt<CarbonProjectDto>())
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting carbon projects: {ex.Message}");
            throw new Exception("Error getting carbon projects.", ex);
        }
    }
}