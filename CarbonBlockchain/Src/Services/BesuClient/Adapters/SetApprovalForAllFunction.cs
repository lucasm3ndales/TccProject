using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("setApprovalForAll")]
public class SetApprovalForAllFunction : FunctionMessage
{
    [Parameter("address", "operator", 1)]
    public string Operator { get; set; }

    [Parameter("bool", "approved", 2)]
    public bool Approved { get; set; }
}