namespace CarbonBlockchain.Services.BesuClient.Dtos;

public class SetApprovalDto
{
    public string AccountAddress { get; set; }
    public string PrivateKey { get; set; }
    public bool IsApproved { get; set; }
}