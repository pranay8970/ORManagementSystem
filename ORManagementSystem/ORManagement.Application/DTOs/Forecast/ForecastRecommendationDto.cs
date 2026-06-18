namespace ORManagement.Application.DTOs.Forecast;

public class ForecastRecommendationDto
{
    public int RecId { get; set; }
    public int HospitalId { get; set; }

    public string RuleId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EvidenceJson { get; set; }

    public string RecStatus { get; set; } = string.Empty;

    public int? ReviewedBy { get; set; }

    public DateTime CreatedAt { get; set; }
}