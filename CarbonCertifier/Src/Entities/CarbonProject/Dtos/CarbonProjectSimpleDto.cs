using CarbonCertifier.Entities.CarbonProject.Enums;

namespace CarbonCertifier.Entities.CarbonProject.Dtos;

public class CarbonProjectSimpleDto
{
    public string ProjectCode { get; set; }

    public string Name { get; set; }

    public string Location { get; set; }
    public string Developer { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }

    public CarbonProjectType Type { get; set; }

    public CarbonProjectStatus Status { get; set; }
}