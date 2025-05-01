using CarbonCertifier.Entities.CreditCarbon.Dtos;
using CarbonCertifier.Services.CarbonCredit;
using CarbonCertifier.Services.WebSocketHosted;
using CarbonCertifier.Services.WebSocketHosted.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarbonCertifier.Controllers.CreditCarbon;

[ApiController]
[Route("v1/carbonCredits")]
public class CreditCarbonController(ICarbonCreditService carbonCreditService, IWebSocketHostedServerService webSocketHostedService) : ControllerBase
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

    /// <summary>
    /// Atualiza os clientes conectados sobre os créditos de carbono e recebe atualizações
    /// </summary>
    [HttpGet("stream")]
    public async Task GetCarbonCreditStreamAsync()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = 400;
            return; 
        }
        
        var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        var webSocketMessageDto = new WebSocketMessageDto(
            200, 
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 
            await carbonCreditService.GetAllAsync());
        
        await webSocketHostedService.ConnectAsync(
            webSocket, 
            webSocketMessageDto, 
            carbonCreditService.HandleWebSocketMessageUpdateAsync);
    }
}