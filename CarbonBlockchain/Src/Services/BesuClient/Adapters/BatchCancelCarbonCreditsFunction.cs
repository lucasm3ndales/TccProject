using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("batchCancelCarbonCredits", typeof(FunctionResponseData))]

public class BatchCancelCarbonCreditsFunction : FunctionMessage
{
    [Parameter("string[]", "creditCodes", 1)]
    public List<string> CreditCodes { get; set; }
}