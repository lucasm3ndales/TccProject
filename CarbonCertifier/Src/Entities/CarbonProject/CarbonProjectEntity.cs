using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarbonCertifier.Entities.CarbonCredit;
using CarbonCertifier.Entities.CarbonProject.Enums;
using Microsoft.EntityFrameworkCore;

namespace CarbonCertifier.Entities.CarbonProject;

[Index(nameof(ProjectCode), IsUnique = true)]
public class CarbonProjectEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string ProjectCode { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
    public string Developer { get; set; }
    public CarbonProjectType Type { get; set; }
    public CarbonProjectStatus Status { get; set; }
    public ICollection<CarbonCreditEntity> CarbonCredits { get; set; } = [];
}


