using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[FunctionOutput]
public class CarbonCreditMetadata
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

    [Parameter("uint256", "createdAt", 7)]
    public BigInteger CreatedAt { get; set; }

    [Parameter("uint256", "updatedAt", 8)]
    public BigInteger UpdatedAt { get; set; }

    [Parameter("string", "projectCode", 9)]
    public string ProjectCode { get; set; }

    [Parameter("string", "projectName", 10)]
    public string ProjectName { get; set; }

    [Parameter("string", "projectLocation", 11)]
    public string ProjectLocation { get; set; }

    [Parameter("string", "projectDeveloper", 12)]
    public string ProjectDeveloper { get; set; }

    [Parameter("uint256", "projectCreatedAt", 13)]
    public BigInteger ProjectCreatedAt { get; set; }

    [Parameter("uint256", "projectUpdatedAt", 14)]
    public BigInteger ProjectUpdatedAt { get; set; }

    [Parameter("string", "projectType", 15)]
    public string ProjectType { get; set; }

    [Parameter("string", "projectStatus", 16)]
    public string ProjectStatus { get; set; }
}