namespace CarbonCertifier.Entities.CarbonProject.Dtos;

public record CarbonProjectCreateDto()
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public string Type { get; set; }
    public string StartDate { get; set; }
    public string? EndDate { get; set; }
    public string Status { get; set; }
    public string Developer { get; set; }
    public string Methodology { get; set; }
    public double EstimateEmissionReductions { get; set; }
    public double? EmissionReductions { get; set; }
    public string? CertificationDate { get; set; }
    public string? CertificationExpiryDate { get; set; }
    public string? CertificationKey { get; set; }
};