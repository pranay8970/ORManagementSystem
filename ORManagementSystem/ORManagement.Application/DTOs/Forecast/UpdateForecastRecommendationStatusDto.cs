using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Forecast;

public class UpdateForecastRecommendationStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;

    [StringLength(300)]
    public string? SchedulerRemarks { get; set; }
}