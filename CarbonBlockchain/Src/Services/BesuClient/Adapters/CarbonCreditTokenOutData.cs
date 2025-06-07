using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[FunctionOutput]
public class CarbonCreditTokenOutData: IFunctionOutputDTO
{
    [Parameter("string", "creditCode", 1)]
    public string CreditCode { get; set; }

    [Parameter("uint256", "vintageYear", 2)]
    public BigInteger VintageYear { get; set; }

    [Parameter("uint256", "tonCO2Quantity", 3)]
    public BigInteger TonCO2Quantity { get; set; }

    [Parameter("string", "status", 4)]
    public string Status { get; set; }

    [Parameter("string", "ownerName", 5)]
    public string OwnerName { get; set; }

    [Parameter("string", "ownerDocument", 6)]
    public string OwnerDocument { get; set; }

    [Parameter("string", "projectCode", 7)]
    public string ProjectCode { get; set; }

    [Parameter("uint256", "createdAt", 8)]
    public BigInteger CreatedAt { get; set; }

    [Parameter("uint256", "updatedAt", 9)]
    public BigInteger UpdatedAt { get; set; }
}