using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarbonCertifier.Entities.CarbonCredit.Enums;
using CarbonCertifier.Entities.CarbonProject;
using Microsoft.EntityFrameworkCore;

namespace CarbonCertifier.Entities.CarbonCredit;

[Index(nameof(CreditCode), IsUnique = true)]
public class CarbonCreditEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string CreditCode { get; set; }
    public int VintageYear { get; set; }
    public double TonCO2Quantity { get; set; }
    public CarbonCreditStatus Status { get; set; }
    public string OwnerName { get; set; }
    public string OwnerDocument { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
    public CarbonProjectEntity CarbonProject { get; set; }
}


