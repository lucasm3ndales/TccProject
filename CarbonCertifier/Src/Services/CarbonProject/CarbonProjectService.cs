using CarbonCertifier.Converters;
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
            carbonProject.StartDate = DateTimeConverter.ConvertStringToDateTime(dto.StartDate);
            
            if (!string.IsNullOrEmpty(dto.EndDate))
            {
                carbonProject.EndDate = DateTimeConverter.ConvertStringToDateTime(dto.EndDate);
            }

            if (!string.IsNullOrEmpty(dto.CertificationDate) &&
                !string.IsNullOrWhiteSpace(dto.CertificationExpiryDate))
            {
                carbonProject.CertificationDate = DateTimeConverter.ConvertStringToDateTime(dto.CertificationDate);
                carbonProject.CertificationExpiryDate = DateTimeConverter.ConvertStringToDateTime(dto.CertificationExpiryDate);
            }
            
            var dbResult = await dbContext.CarbonProjects.AddAsync(carbonProject);
            
            await dbContext.SaveChangesAsync();
            
            _ = Task.Run(async () => await carbonCreditService.SetCarbonCreditsAsync(dbResult.Entity));
            
            return carbonProject.Adapt<CarbonProjectDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating carbon project: {ex.Message}");
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
            dbResult.StartDate = DateTimeConverter.ConvertStringToDateTime(dto.StartDate);

            
            if (!string.IsNullOrEmpty(dto.EndDate))
            {
                dbResult.EndDate = DateTimeConverter.ConvertStringToDateTime(dto.EndDate);
            }

            if (!string.IsNullOrEmpty(dto.CertificationDate) &&
                !string.IsNullOrWhiteSpace(dto.CertificationExpiryDate))
            {
                dbResult.CertificationDate = DateTimeConverter.ConvertStringToDateTime(dto.CertificationDate);
                dbResult.CertificationExpiryDate = DateTimeConverter.ConvertStringToDateTime(dto.CertificationExpiryDate);
            }

            await dbContext.SaveChangesAsync();

            return dbResult.Adapt<CarbonProjectDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating carbon project: {ex.Message}");
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