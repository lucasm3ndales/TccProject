using CarbonCertifier.Entities.CreditCarbon.Dtos;
using CarbonCertifier.Services.CarbonCredit;
using Microsoft.AspNetCore.Mvc;

namespace CarbonCertifier.Controllers.CreditCarbon;

[ApiController]
[Route("v1/carbonCredit")]
public class CreditCarbonController(ICarbonCreditService carbonCreditService) : ControllerBase
{
    /// <summary>
    /// Busca um crédito de carbono específico
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<CarbonCreditDto>> GetCarbonCreditAsync(long id)
    {
        var response = await carbonCreditService.GetByIdAsync(id);
        return Ok(response);
    }
    
    /// <summary>
    /// Busca todos os créditos de carbono
    /// </summary>
    [HttpGet()]
    public async Task<ActionResult<List<CarbonCreditDto>>> GetCarbonCreditsAsync()
    {
        var response = await carbonCreditService.GetAllAsync();
        return Ok(response);
    }
}