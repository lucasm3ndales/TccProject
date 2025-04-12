using CarbonBlockchain.Src.Services.CarbonCreditHandler.Dtos;

namespace CarbonBlockchain.Services.CarbonCreditHandler.Dtos;

public class CarbonCreditCertifierDto
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
    public CarbonProjectCertifierDto CarbonProject { get; set; }
}
