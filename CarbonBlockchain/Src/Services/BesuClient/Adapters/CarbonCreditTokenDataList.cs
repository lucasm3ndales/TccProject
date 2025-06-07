using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[FunctionOutput]
public class CarbonCreditTokenDataList: IFunctionOutputDTO
{
    [Parameter("tuple[]", "", 1)]
    public List<CarbonCreditTokenStructData> CarbonCredits { get; set; }
}