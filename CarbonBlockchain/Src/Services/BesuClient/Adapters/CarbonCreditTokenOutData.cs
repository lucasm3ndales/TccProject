using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[FunctionOutput]
public class CarbonCreditTokenOutData: IFunctionOutputDTO
{
    [Parameter("string", "", 1)]
    public string CreditCode { get; set; }

    [Parameter("uint32", "", 2)]
    public UInt32 VintageYear { get; set; }

    [Parameter("uint32", "", 3)]
    public UInt32 TonCO2Quantity { get; set; }

    [Parameter("string", "", 4)]
    public string Status { get; set; }

    [Parameter("string", "", 5)]
    public string OwnerName { get; set; }

    [Parameter("string", "", 6)]
    public string OwnerDocument { get; set; }
    
    [Parameter("uint64", "", 7)]
    public UInt64 CreatedAt { get; set; }

    [Parameter("uint64", "", 8)]
    public UInt64 UpdatedAt { get; set; }

    [Parameter("string", "", 9)]
    public string ProjectCode { get; set; }
}