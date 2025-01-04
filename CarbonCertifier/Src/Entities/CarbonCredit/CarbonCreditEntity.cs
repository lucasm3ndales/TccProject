
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CarbonCertifier.Entities.CarbonProject;
using Microsoft.EntityFrameworkCore;

namespace CarbonCertifier.Entities.CreditCarbon;

[Index(nameof(CreditCode), IsUnique = true)]
public class CarbonCreditEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public Guid CreditCode { get; set; } = Guid.NewGuid();
    public int VintageYear { get; set; }
    public double TonCO2Quantity { get; set; }
    public bool IsRetired { get; set; }
    public DateTime? RetireAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public double PricePerTon { get; set; }
    public string Owner { get; set; }
    public CarbonProjectEntity CarbonProject { get; set; }
}