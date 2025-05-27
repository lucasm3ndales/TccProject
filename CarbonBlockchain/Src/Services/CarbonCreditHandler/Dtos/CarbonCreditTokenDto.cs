namespace CarbonBlockchain.Services.CarbonCreditHandler.Dtos;

public class CarbonCreditTokenDto
{
    public string CreditCode { get; set; }
    public int VintageYear { get; set; }
    public double TonCO2Quantity { get; set; }
    public string Status { get; set; }
    public string OwnerName { get; set; }
    public string OwnerDocument { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
    public string ProjectCode { get; set; }
    public string ProjectName { get; set; }
    public string ProjectLocation { get; set; }
    public string ProjectDeveloper { get; set; }
    public long ProjectCreatedAt { get; set; }
    public long ProjectUpdatedAt { get; set; }
    public string ProjectType { get; set; }
    public string ProjectStatus { get; set; }
}