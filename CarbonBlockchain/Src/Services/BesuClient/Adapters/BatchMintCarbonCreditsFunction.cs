﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[Function("batchMintCarbonCredits", typeof(ResponseOutputData))]
public class BatchMintCarbonCreditsFunction : FunctionMessage
{
    [Parameter("address", "to", 1)]
    public string To { get; set; }

    [Parameter("tuple[]", "credits", 2)]
    public List<CarbonCreditTokenStructData> Credits { get; set; }
}