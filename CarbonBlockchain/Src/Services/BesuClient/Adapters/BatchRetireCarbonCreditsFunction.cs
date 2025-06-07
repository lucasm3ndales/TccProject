using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("batchRetireCarbonCredits", typeof(ResponseOutputData))]
public class BatchRetireCarbonCreditsFunction : FunctionMessage
{
    [Parameter("string[]", "creditCodes", 1)]
    public List<string> CreditCodes { get; set; }
}