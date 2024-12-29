using CarbonCertifier.Data;
using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CarbonProject.Dtos;
using CarbonCertifier.Services.CarbonCredit;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CarbonCertifier.Services.CarbonProject;

public class CarbonProjectService(CarbonCertifierDbContext dbContext, ICarbonCreditService carbonCreditService) : ICarbonProjectService
{
    public async Task<CarbonProjectDto> CreateAsync(CarbonProjectCreateDto dto)
    {
        try
        {
            var carbonProject = dto.Adapt<CarbonProjectEntity>();

            var dbResult = await dbContext.CarbonProjects.AddAsync(carbonProject);

            
            await carbonCreditService.SetCarbonCreditsAsync(dbResult.Entity);

            await dbContext.SaveChangesAsync();
            
            return carbonProject.Adapt<CarbonProjectDto>();
        }
        catch (Exception ex)
        {
            throw new Exception("Error creating carbon project.", ex);
        }
    }

    public async Task<CarbonProjectDto> UpdateAsync(long id, CarbonProjectUpdateDto dto)
    {
        try
        {
            var dbResult = await dbContext.CarbonProjects.FindAsync(id);

            if (dbResult == null) throw new NullReferenceException("Carbon project not found.");

            dbResult = dto.Adapt<CarbonProjectEntity>();

            await dbContext.SaveChangesAsync();

            return dbResult.Adapt<CarbonProjectDto>();
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating carbon project.", ex);
        }
    }

    public async Task DeleteAsync(long id)
    {
        try
        {
            dbContext.CarbonProjects.Remove(new CarbonProjectEntity { Id = id });

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
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
            throw new Exception("Error getting carbon projects.", ex);
        }
    }
    
  
}