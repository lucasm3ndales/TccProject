using System.Numerics;
using CarbonBlockchain.Services.BesuClient;
using CarbonBlockchain.Services.BesuClient.Adapters;
using CarbonBlockchain.Services.BesuClient.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarbonBlockchain.Controllers.Besu;

[ApiController]
[Route("v1/besu/carbonCredits/token")]
public class BesuController(IBesuClientService besuClientService) : ControllerBase
{
    /// <summary>
    /// Verifica se uma conta é aprovada a tranferir tokens em nome de outra.
    /// </summary>
    [HttpGet("account/approval")]
    public async Task<ActionResult<bool>> IsApprovedForAllAsync([FromQuery] string accountAddress,
        [FromQuery] string privateKey,
        [FromQuery] string operatorAddress)
    {
        var response = await besuClientService.IsApprovedForAllAsync(accountAddress, privateKey, operatorAddress);
        return Ok(response);
    }

    /// <summary>
    /// Aprova uma conta a tranferir tokens em nome de outra.
    /// </summary>
    [HttpPost("account/approval")]
    public async Task<ActionResult<string>> SetApprovalForAllAsync([FromQuery] string accountAddress,
        [FromQuery] string privateKey, [FromQuery] bool isApproved)
    {
        var response = await besuClientService.SetApprovalForAllAsync(accountAddress, privateKey, isApproved);
        return Ok(response);
    }

    /// <summary>
    /// Busca se o token pertence a conta.
    /// </summary>
    [HttpGet("account/balance")]
    public async Task<ActionResult<BigInteger>> GetBalanceOfAsync([FromQuery] string accountAddress,
        [FromQuery] string privateKey, [FromQuery] string creditCode)
    {
        var response = await besuClientService.GetBalanceOfAsync(accountAddress, privateKey, creditCode);
        return Ok(response);
    }

    /// <summary>
    /// Busca um token de credito de carbono pelo creditCode.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CarbonCreditTokenOutData>> GetCarbonCreditTokenDataAsync(string id)
    {
        var response = await besuClientService.GetCarbonCreditTokenDataAsync(id);
        return Ok(response);
    }

    /// <summary>
    /// Transfere tokens de crédito de carbono de uma conta para outra.
    /// </summary>
    [HttpPost("transfer")]
    public async Task<ActionResult<bool>> TransferCarbonCreditTokensInBatchAsync(
        [FromBody] TransferCarbonCreditTokensDto dto)
    {
        var response = await besuClientService.TransferCarbonCreditTokensInBatchAsync(dto);
        return Ok(response);
    }

    /// <summary>
    /// Aposenta créditos de carbono.
    /// </summary>
    [HttpPost("retire")]
    public async Task<ActionResult<bool>> RetireCarbonCreditTokensInBatchAsync([FromBody] List<string> creditCodes)
    {
        var response = await besuClientService.RetireCarbonCreditTokensInBatchAsync(creditCodes);
        return Ok(response);
    }

    /// <summary>
    /// Cancela créditos de carbono.
    /// </summary>
    [HttpPost("cancel")]
    public async Task<ActionResult<bool>> CancelCarbonCreditTokensInBatchAsync([FromBody] List<string> creditCodes)
    {
        var response = await besuClientService.CancelCarbonCreditTokensInBatchAsync(creditCodes);
        return Ok(response);
    }

    /// <summary>
    /// Disponibiliza para venda créditos de carbono.
    /// </summary>
    [HttpPost("available")]
    public async Task<ActionResult<bool>> AvailableCarbonCreditTokensInBatchAsync([FromBody] List<string> creditCodes)
    {
        var response = await besuClientService.AvailableCarbonCreditTokensInBatchAsync(creditCodes);
        return Ok(response);
    }
}