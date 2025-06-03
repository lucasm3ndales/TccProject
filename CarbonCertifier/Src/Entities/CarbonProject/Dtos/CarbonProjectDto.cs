using CarbonCertifier.Entities.CarbonCredit.Dtos;
using CarbonCertifier.Entities.CarbonProject.Enums;

namespace CarbonCertifier.Entities.CarbonProject.Dtos;

public class CarbonProjectDto
{
    public string ProjectCode { get; set; }
    public string Name { get; set; }

    public string Location { get; set; }
    public string Developer { get; set; }
    public long CreatedAt { get; set; }
    public long UpdatedAt { get; set; }

    public CarbonProjectType Type { get; set; }

    public CarbonProjectStatus Status { get; set; }
    public ICollection<CarbonCreditSimpleDto> CarbonCredits { get; set; }

}