using CarbonCertifier.Entities.CarbonCredit;
using CarbonCertifier.Entities.CarbonProject;
using Microsoft.EntityFrameworkCore;

namespace CarbonCertifier.Data;

public class CarbonCertifierDbContext(DbContextOptions<CarbonCertifierDbContext> options) : DbContext(options)
{
    public DbSet<CarbonProjectEntity> CarbonProjects { get; set; }
    public DbSet<CarbonCreditEntity> CarbonCredits { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CarbonCreditEntity>()
            .Property(e => e.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<CarbonProjectEntity>()
            .Property(e => e.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<CarbonProjectEntity>()
            .Property(e => e.Type)
            .HasConversion<string>();

        base.OnModelCreating(modelBuilder);
    }
}