using ORManagement.Application.DTOs.Utilization;

namespace ORManagement.Application.Interfaces.Repositories;

public interface IUtilizationRepository
{
    Task<List<UtilizationRecordDto>> GetUtilizationRecordsAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate,
        int? surgeonId,
        int? roomId,
        string? status);

    Task<UtilizationSummaryDto> GetSummaryAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate);

    Task<List<UnderutilizedBlockDto>> GetUnderutilizedBlocksAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate);

    Task<bool> BlockExistsAsync(int hospitalId, int blockId);

    Task CalculateBlockUtilizationAsync(int blockId);

    Task<List<int>> GetBlockIdsForDateRangeAsync(
        int hospitalId,
        DateTime fromDate,
        DateTime toDate);
}