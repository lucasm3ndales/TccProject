using CarbonBlockchain.Services.CarbonCreditHandler.Enums;

namespace CarbonBlockchain.Src.Services.CarbonCreditHandler.Dtos;

public class CarbonProjectCertifierDto {
        public long Id { get; set; }
    public Guid ProjectCode { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public CarbonProjectType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public CarbonProjectStatus Status { get; set; }
    public bool IsActive { get; set; }
    public string Developer { get; set; }
    public string Methodology { get; set; }
    public double? EstimateEmissionReductions { get; set; }
    public double? EmissionReductions { get; set; }
    public DateTime? CertificationDate { get; set; }
    public DateTime? CertificationExpiryDate { get; set; }
    public string? CertificationKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}