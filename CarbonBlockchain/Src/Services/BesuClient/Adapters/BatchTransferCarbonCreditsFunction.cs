using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("batchTransferCarbonCredits", "tuple")]
public class BatchTransferCarbonCreditsFunction : FunctionMessage
{
    [Parameter("address", "from", 1)]
    public string From { get; set; }

    [Parameter("address", "to", 2)]
    public string To { get; set; }

    [Parameter("string[]", "creditCodes", 3)]
    public List<string> CreditCodes { get; set; }
}