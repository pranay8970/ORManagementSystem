namespace ORManagement.Application.DTOs.Forecast;

public class ForecastDemandSignalDto
{
    public int HospitalId { get; set; }

    public string Specialty { get; set; } = string.Empty;

    public int TotalBlocks { get; set; }
    public int WaitlistedRequests { get; set; }

    public decimal AverageUtilizationPercent { get; set; }
}