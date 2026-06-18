using ORManagement.Application.DTOs.Shared;
using ORManagement.Application.DTOs.Utilization;

namespace ORManagement.Application.Interfaces.Services;

public interface IUtilizationService
{
    Task<ServiceResultDto<List<UtilizationRecordDto>>> GetUtilizationRecordsAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate,
        int? surgeonId,
        int? roomId,
        string? status);

    Task<ServiceResultDto<UtilizationSummaryDto>> GetSummaryAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate);

    Task<ServiceResultDto<List<UnderutilizedBlockDto>>> GetUnderutilizedBlocksAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate);

    Task<ServiceResultDto<int>> CalculateUtilizationAsync(
        int hospitalId,
        int userId,
        string roleName,
        CalculateUtilizationRequestDto request,
        string? ipAddress,
        string? userAgent);
}