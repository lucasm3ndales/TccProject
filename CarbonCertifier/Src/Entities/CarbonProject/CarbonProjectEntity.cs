
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarbonCertifier.Entities.CarbonProject.Enums;
using CarbonCertifier.Entities.CreditCarbon;
using Microsoft.EntityFrameworkCore;

namespace CarbonCertifier.Entities.CarbonProject;

[Index(nameof(ProjectCode), IsUnique = true)]
public class CarbonProjectEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public Guid ProjectCode { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    [MaxLength(500)]
    public string Description { get; set; }
    public string Location { get; set; }
    public CarbonProjectType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public CarbonProjectStatus Status { get; set; }
    public bool IsActive { get; set; } = true;
    public string Developer { get; set; }
    public string Methodology { get; set; }
    public double? EstimateEmissionReductions { get; set; }
    public double? EmissionReductions { get; set; }
    public DateTime? CertificationDate { get; set; }
    public DateTime? CertificationExpiryDate { get; set; }
    public string? CertificationKey { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ModifiedAt { get; set; } = DateTime.Now;
    public ICollection<CarbonCreditEntity> CarbonCredits { get; set; } = [];
}


