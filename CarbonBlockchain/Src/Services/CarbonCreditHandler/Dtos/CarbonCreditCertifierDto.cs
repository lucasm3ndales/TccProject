using CarbonBlockchain.Services.CarbonCreditHandler.Enums;

namespace CarbonBlockchain.Services.CarbonCreditHandler.Dtos;

public class CarbonCreditCertifierDto
{
    public string CreditCode { get; set; }
    public int VintageYear { get; set; }
    public double TonCO2Quantity { get; set; }
    public CarbonCreditStatus Status { get; set; }
    public string OwnerName { get; set; }
    public string OwnerDocument { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
    public string ProjectCode { get; set; }
}
