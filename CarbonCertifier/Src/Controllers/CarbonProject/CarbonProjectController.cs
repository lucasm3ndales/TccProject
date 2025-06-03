using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CarbonProject.Dtos;
using CarbonCertifier.Services.CarbonProject;
using Microsoft.AspNetCore.Mvc;

namespace CarbonCertifier.Controllers.CarbonProject;

[ApiController]
[Route("v1/carbonProjects")]
public class CarbonProjectController(ICarbonProjectService carbonProjectService) : ControllerBase
{

    /// <summary>
    /// Cria um projeto de carbono
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CarbonProjectEntity>> CreateCarbonProjectAsync([FromBody] CarbonProjectCreateDto dto)
    {
        var response = await carbonProjectService.CreateAsync(dto);
        return Created("", response);
    }
    
    /// <summary>
    /// Atualiza um projeto de carbono
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<CarbonProjectEntity>> UpdateCarbonProjectAsync(long id, [FromBody] CarbonProjectUpdateDto dto)
    {
        var response = await carbonProjectService.UpdateAsync(id, dto);
        return Ok(response);
    }
    
    /// <summary>
    /// Deleta um projeto de carbono
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult> DeleteCarbonProjectAsync(long id)
    {
        await carbonProjectService.DeleteAsync(id);
        return NoContent();
    }
    
    /// <summary>
    /// Busca todos os projetos de carbono
    /// </summary>
    [HttpGet()]
    public async Task<ActionResult<List<CarbonProjectEntity>>> GetAllCarbonProjectsAsync()
    {
        var response = await carbonProjectService.GetAllAsync();
        return Ok(response);
    }
    
    /// <summary>
    /// Busca um projeto de carbono específico
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<CarbonProjectEntity>> GetCarbonProjectAsync(long id)
    {
        var response = await carbonProjectService.GetByIdAsync(id);
        return Ok(response);
    }
}