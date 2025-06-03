using CarbonBlockchain.Entities.Accounts.Dtos;

namespace CarbonBlockchain.Services.Account;

public interface IAccountService
{
    Task<AccountDto> CreateAsync(AccountCreateDto dto);
    Task<AccountDto> UpdateAsync(long id, AccountUpdateDto dto);
    Task DeleteAsync(long id);
    Task<AccountDto> GetByIdAsync(long id);

}