namespace CarbonBlockchain.Services.BesuClient.Dtos;

public class TransferCarbonCreditTokensDto
{
    public string From { get; set; }
    public string To { get; set; }
    public List<string> CreditCodes { get; set; }
}