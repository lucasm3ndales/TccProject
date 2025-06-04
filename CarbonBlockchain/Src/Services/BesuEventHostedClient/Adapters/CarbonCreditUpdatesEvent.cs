using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CarbonBlockchain.Services.BesuEventHostedClient.Adapters;

[Event("CarbonCreditUpdates")]
public class CarbonCreditUpdatesEvent: EventDTO
{
    [Parameter("address", "operator", 1, true)]
    public string Operator { get; set; }

    [Parameter("string", "func", 2, false)]
    public string Func { get; set; }

    [Parameter("uint256[]", "tokenIds", 3, false)]
    public List<BigInteger> TokenIds { get; set; }

    [Parameter("string[]", "creditCodes", 4, false)]
    public List<string> CreditCodes { get; set; }
}