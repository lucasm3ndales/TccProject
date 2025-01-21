using Microsoft.EntityFrameworkCore;

namespace CarbonBlockchain.Data;

public class CarbonBlockchainDbContext(DbContextOptions<CarbonBlockchainDbContext> options): DbContext(options)
{
    
}