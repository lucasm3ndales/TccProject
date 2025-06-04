﻿using System.Numerics;
using CarbonBlockchain.Services.CarbonCreditHandler.Enums;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[FunctionOutput]
public class CarbonCreditTokenData: FunctionOutputDTO
{
    [Parameter("string", "creditCode", 1)]
    public string CreditCode { get; set; }

    [Parameter("uint256", "vintageYear", 2)]
    public BigInteger VintageYear { get; set; }

    [Parameter("uint256", "tonCO2Quantity", 3)]
    public BigInteger TonCO2Quantity { get; set; }

    [Parameter("uint8", "status", 4)]
    public byte Status { get; set; }

    [Parameter("string", "ownerName", 5)]
    public string OwnerName { get; set; }

    [Parameter("string", "ownerDocument", 6)]
    public string OwnerDocument { get; set; }

    [Parameter("uint256", "createdAt", 7)]
    public BigInteger CreatedAt { get; set; }

    [Parameter("uint256", "updatedAt", 8)]
    public BigInteger UpdatedAt { get; set; }

    [Parameter("string", "projectCode", 9)]
    public string ProjectCode { get; set; }
}