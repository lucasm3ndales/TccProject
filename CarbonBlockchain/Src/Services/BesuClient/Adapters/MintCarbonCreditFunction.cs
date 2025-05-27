using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("mintCarbonCredit", "uint256")]
public class MintCarbonCreditFunction: FunctionMessage
{
    [Parameter("address", "to", 1)]
    public string To { get; set; }

    [Parameter("tuple", "data", 2)]
    public CarbonCreditMetadata Data { get; set; }

    [Parameter("string", "message", 3)]
    public string Message { get; set; }
}

