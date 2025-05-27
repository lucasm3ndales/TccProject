using CarbonBlockchain.Services.CarbonCreditHandler.Enums;

namespace CarbonBlockchain.Src.Services.CarbonCreditHandler.Dtos;

public class CarbonProjectCertifierDto {
    public string ProjectCode { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Developer { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }
    public CarbonProjectType Type { get; set; }
    public CarbonProjectStatus Status { get; set; }
}