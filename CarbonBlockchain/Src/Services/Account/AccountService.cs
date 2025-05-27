using CarbonBlockchain.Data;
using CarbonBlockchain.Entities.Account;
using CarbonBlockchain.Entities.Account.Dtos;
using Mapster;

namespace CarbonBlockchain.Services.Account;

public class AccountService(CarbonBlockchainDbContext dbContext): IAccountService
{
    public async Task<AccountDto> CreateAsync(AccountCreateDto dto)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var accountEntity = dto.Adapt<AccountEntity>();
            
            var dbResult = await dbContext.Accounts.AddAsync(accountEntity);
            
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return dbResult.Entity.Adapt<AccountDto>();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating ethereum account: {ex.Message}");
            throw new Exception("Error creating ethereum account.", ex);
        }
    }

    public async Task<AccountDto> UpdateAsync(long id, AccountUpdateDto dto)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var dbResult = await dbContext.Accounts.FindAsync(id);

            if (dbResult == null) throw new NullReferenceException("Ethereum account not found.");

            dbResult = dto.Adapt<AccountEntity>();

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return dbResult.Adapt<AccountDto>();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating ethereum account: {ex.Message}");
            throw new Exception("Error updating ethereum account.", ex);
        }
    }

    public async Task DeleteAsync(long id)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var dbResult = await dbContext.Accounts.FindAsync(id);

            if (dbResult == null) throw new NullReferenceException("Ethereum account not found.");
            
            dbContext.Accounts.Remove(dbResult);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error deleting ethereum account: {ex.Message}");
            throw new Exception("Error deleting ethereum account.", ex);
        }
    }

    public async Task<AccountDto> GetByIdAsync(long id)
    {
        try
        {
            var dbResult = await dbContext.Accounts.FindAsync(id);

            if (dbResult == null) throw new NullReferenceException("Ethereum account not found.");

            return dbResult.Adapt<AccountDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting ethereum account: {ex.Message}");
            throw new Exception("Error getting ethereum account.", ex);
        }
    }
}