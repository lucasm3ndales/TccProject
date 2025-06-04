using CarbonBlockchain.Services.BesuClient;
using CarbonBlockchain.Services.BesuClient.Adapters;
using CarbonBlockchain.Services.BesuClient.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarbonBlockchain.Controllers.Besu;

[ApiController]
[Route("v1/besu")]
public class BesuController(IBesuClientService besuClientService): ControllerBase
{
    /// <summary>
    /// Busca um token de credito de carbono pelo creditCode.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CarbonCreditTokenData>> GetCarbonCreditTokenDataAsync(string id)
    {
        var response = await besuClientService.GetCarbonCreditTokenDataAsync(id);
        return Ok(response);
    }
    
    /// <summary>
    /// Transfere tokens de crédito de carbono de uma conta para outra.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<bool>> TransferCarbonCreditTokensInBatchAsync([FromBody] TransferCarbonCreditTokensDto dto)
    {
        var response = await besuClientService.TransferCarbonCreditTokensInBatchAsync(dto);
        return Ok(response);
    }
    
    /// <summary>
    /// Aposenta créditos de carbono.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<bool>> RetireCarbonCreditTokensInBatchAsync([FromBody] List<string> creditCodes)
    {
        var response = await besuClientService.RetireCarbonCreditTokensInBatchAsync(creditCodes);
        return Ok(response);
    }
    
    /// <summary>
    /// Cancela créditos de carbono.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<bool>> CancelCarbonCreditTokensInBatchAsync([FromBody] List<string> creditCodes)
    {
        var response = await besuClientService.CancelCarbonCreditTokensInBatchAsync(creditCodes);
        return Ok(response);
    }
    
    /// <summary>
    /// Disponibiliza para venda créditos de carbono.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<bool>> AvailableCarbonCreditTokensInBatchAsync([FromBody] List<string> creditCodes)
    {
        var response = await besuClientService.AvailableCarbonCreditTokensInBatchAsync(creditCodes);
        return Ok(response);
    }
}