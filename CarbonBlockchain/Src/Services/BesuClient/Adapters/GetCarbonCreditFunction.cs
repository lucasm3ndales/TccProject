using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("getCarbonCredit", typeof(CarbonCreditTokenOutData))]
public class GetCarbonCreditFunction: FunctionMessage
{
    [Parameter("string", "creditCode", 1)]
    public string CreditCode { get; set; }
}