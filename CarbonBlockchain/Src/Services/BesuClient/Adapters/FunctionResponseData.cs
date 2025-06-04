﻿using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[FunctionOutput]
public class FunctionResponseData
{
    [Parameter("bool", "", 1)]
    public bool Success { get; set; }

    [Parameter("string", "", 2)]
    public string Message { get; set; }
}