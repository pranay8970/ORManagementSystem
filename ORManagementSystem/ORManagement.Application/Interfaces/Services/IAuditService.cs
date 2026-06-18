using ORManagement.Application.DTOs.Audit;
using ORManagement.Application.DTOs.Shared;

namespace ORManagement.Application.Interfaces.Services;

public interface IAuditService
{
    Task<ServiceResultDto<List<AuditLogDto>>> GetAuditLogsAsync(
        int hospitalId,
        string? entity,
        string? action,
        DateTime? fromDate,
        DateTime? toDate);

    Task<ServiceResultDto<List<PhiAccessLogDto>>> GetPhiAccessLogsAsync(
        int hospitalId,
        int? patientId,
        int? userId,
        DateTime? fromDate,
        DateTime? toDate);
}