using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("isApprovedForAll", "bool")]
public class IsApprovedForAllFunction : FunctionMessage
{
    [Parameter("address", "account", 1)]
    public string Account { get; set; }

    [Parameter("address", "operator", 2)]
    public string Operator { get; set; }
}