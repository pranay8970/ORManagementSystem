using System.Text.Json;
using ORManagement.Application.DTOs.Forecast;

namespace ORManagement.Application.Engines;

public class ForecastRecommendationEngine
{
    public List<CreateForecastRecommendationDto> GenerateRecommendations(
        List<ForecastDemandSignalDto> demandSignals)
    {
        var recommendations = new List<CreateForecastRecommendationDto>();

        foreach (var signal in demandSignals)
        {
            if (signal.WaitlistedRequests >= 3 && signal.AverageUtilizationPercent >= 80)
            {
                recommendations.Add(new CreateForecastRecommendationDto
                {
                    HospitalId = signal.HospitalId,
                    RuleId = "INC_BLOCK",
                    Description = $"High demand detected for {signal.Specialty}. Consider increasing future block allocation.",
                    EvidenceJson = JsonSerializer.Serialize(new
                    {
                        signal.Specialty,
                        signal.WaitlistedRequests,
                        signal.AverageUtilizationPercent,
                        signal.TotalBlocks,
                        RecommendationType = "IncreaseBlockTime"
                    })
                });
            }

            if (signal.AverageUtilizationPercent < 50 && signal.TotalBlocks >= 2)
            {
                recommendations.Add(new CreateForecastRecommendationDto
                {
                    HospitalId = signal.HospitalId,
                    RuleId = "RED_BLOCK",
                    Description = $"Low utilization detected for {signal.Specialty}. Consider reducing or reallocating future block time.",
                    EvidenceJson = JsonSerializer.Serialize(new
                    {
                        signal.Specialty,
                        signal.WaitlistedRequests,
                        signal.AverageUtilizationPercent,
                        signal.TotalBlocks,
                        RecommendationType = "ReduceBlockTime"
                    })
                });
            }
        }

        return recommendations;
    }
}