using Microsoft.EntityFrameworkCore;
using ORManagement.Application.DTOs.Forecast;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Infrastructure.Data;
using ORManagement.Infrastructure.Data.Entities;

namespace ORManagement.Infrastructure.Repositories;

public class ForecastRepository : IForecastRepository
{
    private readonly ORManagementDbContext _dbContext;

    public ForecastRepository(ORManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ForecastRecommendationDto>> GetRecommendationsAsync(
        int hospitalId,
        string? status)
    {
        var query = _dbContext.ForecastRecommendations
            .Where(recommendation => recommendation.HospitalId == hospitalId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(recommendation => recommendation.RecStatus == status);
        }

        return await query
            .OrderByDescending(recommendation => recommendation.CreatedAt)
            .Select(recommendation => new ForecastRecommendationDto
            {
                RecId = recommendation.RecId,
                HospitalId = recommendation.HospitalId,
                RuleId = recommendation.RuleId,
                Description = recommendation.Description,
                EvidenceJson = recommendation.EvidenceJson,
                RecStatus = recommendation.RecStatus,
                ReviewedBy = recommendation.ReviewedBy,
                CreatedAt = recommendation.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ForecastRecommendationDto?> GetRecommendationByIdAsync(
        int hospitalId,
        int recId)
    {
        return await _dbContext.ForecastRecommendations
            .Where(recommendation =>
                recommendation.HospitalId == hospitalId &&
                recommendation.RecId == recId)
            .Select(recommendation => new ForecastRecommendationDto
            {
                RecId = recommendation.RecId,
                HospitalId = recommendation.HospitalId,
                RuleId = recommendation.RuleId,
                Description = recommendation.Description,
                EvidenceJson = recommendation.EvidenceJson,
                RecStatus = recommendation.RecStatus,
                ReviewedBy = recommendation.ReviewedBy,
                CreatedAt = recommendation.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<ForecastDemandSignalDto>> GetDemandSignalsAsync(int hospitalId)
    {
        var blockSignals = await
            (
                from block in _dbContext.BlockAllocations
                join surgeon in _dbContext.Surgeons on block.SurgeonId equals surgeon.SurgeonId
                where block.HospitalId == hospitalId &&
                      block.BlockStatus != "Cancelled"
                group block by surgeon.Specialty into groupData
                select new
                {
                    Specialty = groupData.Key,
                    TotalBlocks = groupData.Count()
                }
            )
            .ToListAsync();

        var utilizationSignals = await
            (
                from utilization in _dbContext.UtilizationRecords
                join block in _dbContext.BlockAllocations on utilization.BlockId equals block.BlockId
                join surgeon in _dbContext.Surgeons on block.SurgeonId equals surgeon.SurgeonId
                where block.HospitalId == hospitalId
                group utilization by surgeon.Specialty into groupData
                select new
                {
                    Specialty = groupData.Key,
                    AverageUtilizationPercent = groupData.Average(item => item.UtilizationPct ?? 0)
                }
            )
            .ToListAsync();

        var waitlistSignals = await
            (
                from waitlist in _dbContext.WaitlistRequests
                join request in _dbContext.ORRequests on waitlist.RequestId equals request.RequestId
                join surgeon in _dbContext.Surgeons on request.SurgeonId equals surgeon.SurgeonId
                where request.HospitalId == hospitalId &&
                      request.RequestStatus == "Waitlisted"
                group waitlist by surgeon.Specialty into groupData
                select new
                {
                    Specialty = groupData.Key,
                    WaitlistedRequests = groupData.Count()
                }
            )
            .ToListAsync();

        var specialties = blockSignals
            .Select(item => item.Specialty)
            .Union(utilizationSignals.Select(item => item.Specialty))
            .Union(waitlistSignals.Select(item => item.Specialty))
            .Distinct()
            .ToList();

        return specialties
            .Select(specialty => new ForecastDemandSignalDto
            {
                HospitalId = hospitalId,
                Specialty = specialty,
                TotalBlocks = blockSignals.FirstOrDefault(item => item.Specialty == specialty)?.TotalBlocks ?? 0,
                AverageUtilizationPercent = Math.Round(
                    utilizationSignals.FirstOrDefault(item => item.Specialty == specialty)?.AverageUtilizationPercent ?? 0,
                    2),
                WaitlistedRequests = waitlistSignals.FirstOrDefault(item => item.Specialty == specialty)?.WaitlistedRequests ?? 0
            })
            .ToList();
    }

    public async Task<int> CreateRecommendationAsync(CreateForecastRecommendationDto request)
    {
        var recommendation = new ForecastRecommendation
        {
            HospitalId = request.HospitalId,
            RuleId = request.RuleId,
            Description = request.Description,
            EvidenceJson = request.EvidenceJson,
            RecStatus = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.ForecastRecommendations.AddAsync(recommendation);
        await _dbContext.SaveChangesAsync();

        return recommendation.RecId;
    }

    public async Task<bool> UpdateRecommendationStatusAsync(
        int hospitalId,
        int recId,
        string status,
        int reviewedByUserId)
    {
        var recommendation = await _dbContext.ForecastRecommendations
            .FirstOrDefaultAsync(recommendation =>
                recommendation.HospitalId == hospitalId &&
                recommendation.RecId == recId);

        if (recommendation is null)
        {
            return false;
        }

        recommendation.RecStatus = status;
        recommendation.ReviewedBy = reviewedByUserId;

        await _dbContext.SaveChangesAsync();

        return true;
    }
}