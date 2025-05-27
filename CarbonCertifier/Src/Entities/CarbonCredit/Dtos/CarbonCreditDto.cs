using CarbonCertifier.Entities.CarbonProject.Dtos;
using CarbonCertifier.Entities.CreditCarbon.Enums;

namespace CarbonCertifier.Entities.CreditCarbon.Dtos;

public class CarbonCreditDto()
{
    public string CreditCode { get; set; }
    public int VintageYear { get; set; }
    public double TonCO2Quantity { get; set; }
    public CarbonCreditStatus Status { get; set; }
    public string OwnerName { get; set; }
    public string OwnerDocument { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
    public CarbonProjectSimpleDto CarbonProject { get; set; }
};