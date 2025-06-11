using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("batchTransferCarbonCredits", typeof(ResponseOutputData))]
public class BatchTransferCarbonCreditsFunction : FunctionMessage
{
    [Parameter("address", "from", 1)]
    public string From { get; set; }

    [Parameter("address", "to", 2)]
    public string To { get; set; }

    [Parameter("string[]", "creditCodes", 3)]
    public List<string> CreditCodes { get; set; }
    
    [Parameter("string", "ownerName", 4)]
    public string OwnerName { get; set; }
    
    [Parameter("string", "ownerDocument", 5)]
    public string OwnerDocument{ get; set; }
}