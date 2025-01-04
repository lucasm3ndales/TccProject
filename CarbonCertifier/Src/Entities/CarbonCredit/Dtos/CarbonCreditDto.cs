using CarbonCertifier.Entities.CarbonProject.Dtos;

namespace CarbonCertifier.Entities.CreditCarbon.Dtos;

public record CarbonCreditDto()
{
    public long Id { get; set; }
    public Guid CreditCode { get; set; }
    public int VintageYear { get; set; }
    public double TonCO2Quantity { get; set; }
    public bool IsRetired { get; set; }
    public DateTime? RetireAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public double PricePerTon { get; set; }
    public string Owner { get; set; }
    public CarbonProjectDto CarbonProject { get; set; }
};