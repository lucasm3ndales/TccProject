using CarbonBlockchain.Entities.Account;
using Microsoft.EntityFrameworkCore;

namespace CarbonBlockchain.Data;

public class CarbonBlockchainDbContext(DbContextOptions<CarbonBlockchainDbContext> options): DbContext(options)
{
    public DbSet<AccountEntity> Accounts { get; set; }
}