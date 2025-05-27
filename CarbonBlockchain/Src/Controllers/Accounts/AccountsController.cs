using CarbonBlockchain.Entities.Account.Dtos;
using CarbonBlockchain.Services.Account;
using Microsoft.AspNetCore.Mvc;

namespace CarbonBlockchain.Controllers.Accounts;

[ApiController]
[Route("v1/accounts")]
public class AccountsController(IAccountService accountService): ControllerBase
{
    /// <summary>
    /// Adiciona uma representação de conta Ethereum
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] AccountCreateDto dto)
    {
        var response = await accountService.CreateAsync(dto);
        return Created("", response);
    }
    
    /// <summary>
    /// Atualiza uma representação de conta Ethereum
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<ActionResult<AccountDto>> UpdateAccount(long id, [FromBody] AccountUpdateDto dto)
    {
        var response = await accountService.UpdateAsync(id, dto);
        return Ok(response);
    }
    
    /// <summary>
    /// Deleta uma representação de conta Ethereum
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult<AccountDto>> DeleteAccount(long id)
    {
        await accountService.DeleteAsync(id);
        return NoContent();
    }
    
    /// <summary>
    /// Busca uma representação de conta Ethereum
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<AccountDto>> GetAccountById(long id)
    {
        var response = await accountService.GetByIdAsync(id);
        return Ok(response);
    }
}