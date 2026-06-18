using Microsoft.Extensions.Logging;
using ORManagement.Application.DTOs.Audit;
using ORManagement.Application.DTOs.Shared;
using ORManagement.Application.DTOs.Utilization;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Application.Services;

public class UtilizationService : IUtilizationService
{
    private readonly IUtilizationRepository _utilizationRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly ILogger<UtilizationService> _logger;

    public UtilizationService(
        IUtilizationRepository utilizationRepository,
        IAuditRepository auditRepository,
        ILogger<UtilizationService> logger)
    {
        _utilizationRepository = utilizationRepository;
        _auditRepository = auditRepository;
        _logger = logger;
    }

    public async Task<ServiceResultDto<List<UtilizationRecordDto>>> GetUtilizationRecordsAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate,
        int? surgeonId,
        int? roomId,
        string? status)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
        {
            return ServiceResultDto<List<UtilizationRecordDto>>.Fail(
                "INVALID_DATE_RANGE",
                "From date cannot be after To date.");
        }

        var records = await _utilizationRepository.GetUtilizationRecordsAsync(
            hospitalId,
            fromDate,
            toDate,
            surgeonId,
            roomId,
            status);

        return ServiceResultDto<List<UtilizationRecordDto>>.Ok(records);
    }

    public async Task<ServiceResultDto<UtilizationSummaryDto>> GetSummaryAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
        {
            return ServiceResultDto<UtilizationSummaryDto>.Fail(
                "INVALID_DATE_RANGE",
                "From date cannot be after To date.");
        }

        var summary = await _utilizationRepository.GetSummaryAsync(
            hospitalId,
            fromDate,
            toDate);

        return ServiceResultDto<UtilizationSummaryDto>.Ok(summary);
    }

    public async Task<ServiceResultDto<List<UnderutilizedBlockDto>>> GetUnderutilizedBlocksAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
        {
            return ServiceResultDto<List<UnderutilizedBlockDto>>.Fail(
                "INVALID_DATE_RANGE",
                "From date cannot be after To date.");
        }

        var blocks = await _utilizationRepository.GetUnderutilizedBlocksAsync(
            hospitalId,
            fromDate,
            toDate);

        return ServiceResultDto<List<UnderutilizedBlockDto>>.Ok(blocks);
    }

    public async Task<ServiceResultDto<int>> CalculateUtilizationAsync(
        int hospitalId,
        int userId,
        string roleName,
        CalculateUtilizationRequestDto request,
        string? ipAddress,
        string? userAgent)
    {
        var calculatedCount = 0;

        if (request.BlockId.HasValue)
        {
            var exists = await _utilizationRepository.BlockExistsAsync(
                hospitalId,
                request.BlockId.Value);

            if (!exists)
            {
                return ServiceResultDto<int>.Fail(
                    "BLOCK_NOT_FOUND",
                    "Block was not found.");
            }

            await _utilizationRepository.CalculateBlockUtilizationAsync(request.BlockId.Value);
            calculatedCount = 1;
        }
        else
        {
            if (!request.FromDate.HasValue || !request.ToDate.HasValue)
            {
                return ServiceResultDto<int>.Fail(
                    "DATE_RANGE_REQUIRED",
                    "FromDate and ToDate are required when BlockId is not provided.");
            }

            if (request.FromDate.Value.Date > request.ToDate.Value.Date)
            {
                return ServiceResultDto<int>.Fail(
                    "INVALID_DATE_RANGE",
                    "From date cannot be after To date.");
            }

            var blockIds = await _utilizationRepository.GetBlockIdsForDateRangeAsync(
                hospitalId,
                request.FromDate.Value,
                request.ToDate.Value);

            foreach (var blockId in blockIds)
            {
                await _utilizationRepository.CalculateBlockUtilizationAsync(blockId);
                calculatedCount++;
            }
        }

        await _auditRepository.AddAuditLogAsync(new CreateAuditLogDto
        {
            HospitalId = hospitalId,
            UserId = userId,
            RoleName = roleName,
            Action = "UtilizationCalculated",
            Entity = "UtilizationRecords",
            EntityId = request.BlockId,
            NewValue = calculatedCount.ToString(),
            Remarks = request.BlockId.HasValue
                ? $"Utilization calculated for block {request.BlockId.Value}."
                : $"Utilization calculated for range {request.FromDate:yyyy-MM-dd} to {request.ToDate:yyyy-MM-dd}.",
            IpAddress = ipAddress,
            UserAgent = userAgent
        });

        _logger.LogInformation(
            "Utilization calculated. Count: {CalculatedCount}, UserId: {UserId}",
            calculatedCount,
            userId);

        return ServiceResultDto<int>.Ok(
            calculatedCount,
            "Utilization calculation completed successfully.");
    }
}