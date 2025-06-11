namespace CarbonBlockchain.Services.BesuClient.Dtos;

public class TransferCarbonCreditTokensDto
{
    public string ownerName { get; set; }
    public string ownerDocument { get; set; }
    public string From { get; set; }
    public string PrivateKey { get; set; }
    public string To { get; set; }
    public List<string> CreditCodes { get; set; }
}