using Microsoft.Extensions.Logging;
using ORManagement.Application.DTOs.Audit;
using ORManagement.Application.DTOs.Shared;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Application.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepository;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IAuditRepository auditRepository,
        ILogger<AuditService> logger)
    {
        _auditRepository = auditRepository;
        _logger = logger;
    }

    public async Task<ServiceResultDto<List<AuditLogDto>>> GetAuditLogsAsync(
        int hospitalId,
        string? entity,
        string? action,
        DateTime? fromDate,
        DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
        {
            return ServiceResultDto<List<AuditLogDto>>.Fail(
                "INVALID_DATE_RANGE",
                "From date cannot be after To date.");
        }

        var logs = await _auditRepository.GetAuditLogsAsync(
            hospitalId,
            entity,
            action,
            fromDate,
            toDate);

        return ServiceResultDto<List<AuditLogDto>>.Ok(logs);
    }

    public async Task<ServiceResultDto<List<PhiAccessLogDto>>> GetPhiAccessLogsAsync(
        int hospitalId,
        int? patientId,
        int? userId,
        DateTime? fromDate,
        DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
        {
            return ServiceResultDto<List<PhiAccessLogDto>>.Fail(
                "INVALID_DATE_RANGE",
                "From date cannot be after To date.");
        }

        var logs = await _auditRepository.GetPhiAccessLogsAsync(
            hospitalId,
            patientId,
            userId,
            fromDate,
            toDate);

        return ServiceResultDto<List<PhiAccessLogDto>>.Ok(logs);
    }
}