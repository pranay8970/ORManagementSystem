namespace ORManagement.Application.DTOs.Forecast;

public class CreateForecastRecommendationDto
{
    public int HospitalId { get; set; }

    public string RuleId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EvidenceJson { get; set; }
}