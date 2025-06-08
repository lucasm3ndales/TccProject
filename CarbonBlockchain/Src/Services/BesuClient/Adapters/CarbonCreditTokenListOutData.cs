using Nethereum.ABI.FunctionEncoding.Attributes;

namespace CarbonBlockchain.Services.BesuClient.Adapters;

[FunctionOutput]
public class CarbonCreditTokenListOutData: IFunctionOutputDTO
{
    [Parameter("string[]", "", 1)]
    public List<string> CreditCodes { get; set; }

    [Parameter("uint32[]", "", 2)]
    public List<uint> VintageYears { get; set; }

    [Parameter("uint32[]", "", 3)]
    public List<uint> TonCO2Quantities { get; set; }

    [Parameter("string[]", "", 4)]
    public List<string> Statuses { get; set; }

    [Parameter("string[]", "", 5)]
    public List<string> OwnerNames { get; set; }

    [Parameter("string[]", "", 6)]
    public List<string> OwnerDocuments { get; set; }

    [Parameter("uint64[]", "", 7)]
    public List<ulong> CreatedAts { get; set; }

    [Parameter("uint64[]", "", 8)]
    public List<ulong> UpdatedAts { get; set; }

    [Parameter("string[]", "", 9)]
    public List<string> ProjectCodes { get; set; }
}