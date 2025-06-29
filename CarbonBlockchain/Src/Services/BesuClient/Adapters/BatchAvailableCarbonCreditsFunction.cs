using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("batchAvailableCarbonCredits", typeof(ResponseOutputData))]

public class BatchAvailableCarbonCreditsFunction: FunctionMessage
{
    [Parameter("string[]", "creditCodes", 1)]
    public List<string> CreditCodes { get; set; }
}