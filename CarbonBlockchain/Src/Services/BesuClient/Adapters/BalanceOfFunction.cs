﻿using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("balanceOf", "uint256")]
public class BalanceOfFunction : FunctionMessage
{
    [Parameter("address", "account", 1)]
    public string Account { get; set; }

    [Parameter("string", "creditCode", 2)]
    public string CreditCode { get; set; }
}