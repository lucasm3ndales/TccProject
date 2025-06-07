using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Struct("CarbonCreditTokenData")]
public class CarbonCreditTokenStructData
{
    [Parameter("string", "creditCode", 1)]
    public string CreditCode { get; set; }

    [Parameter("uint256", "vintageYear", 2)]
    public BigInteger VintageYear { get; set; }

    [Parameter("uint256", "tonCO2Quantity", 3)]
    public BigInteger TonCO2Quantity { get; set; }

    [Parameter("string", "status", 4)]
    public string  Status { get; set; }

    [Parameter("string", "ownerName", 5)]
    public string OwnerName { get; set; }

    [Parameter("string", "ownerDocument", 6)]
    public string OwnerDocument { get; set; }
    [Parameter("uint256", "ownerName", 7)]
    public BigInteger CreatedAt { get; set; }

    [Parameter("uint256", "ownerDocument", 8)]
    public BigInteger UpdatedAt { get; set; }

    [Parameter("string", "projectCode", 9)]
    public string ProjectCode { get; set; }
}