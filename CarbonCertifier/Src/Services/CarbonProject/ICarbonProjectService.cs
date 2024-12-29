using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CarbonProject.Dtos;

namespace CarbonCertifier.Services.CarbonProject;

public interface ICarbonProjectService
{
    Task<CarbonProjectDto> CreateAsync (CarbonProjectCreateDto dto);
    Task<CarbonProjectDto> UpdateAsync (long id, CarbonProjectUpdateDto dto);
    Task DeleteAsync (long id);
    Task<CarbonProjectDto> GetByIdAsync (long id);
    Task<List<CarbonProjectDto>> GetAllAsync ();

}