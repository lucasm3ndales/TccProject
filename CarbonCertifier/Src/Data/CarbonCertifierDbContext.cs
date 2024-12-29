using CarbonCertifier.Entities.CarbonProject;
using CarbonCertifier.Entities.CreditCarbon;
using Microsoft.EntityFrameworkCore;

namespace CarbonCertifier.Data;

public class CarbonCertifierDbContext(DbContextOptions<CarbonCertifierDbContext> options) : DbContext(options)
{
    public DbSet<CarbonProjectEntity> CarbonProjects { get; set; }
    public DbSet<CarbonCreditEntity> CarbonCredits { get; set; }
}