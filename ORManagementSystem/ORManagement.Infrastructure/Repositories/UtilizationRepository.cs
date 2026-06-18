using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ORManagement.Application.DTOs.Utilization;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Infrastructure.Data;
using ORManagement.Infrastructure.Data.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ORManagement.Infrastructure.Repositories;

public class UtilizationRepository : IUtilizationRepository
{
    private readonly ORManagementDbContext _dbContext;

    public UtilizationRepository(ORManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<UtilizationRecordDto>> GetUtilizationRecordsAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate,
        int? surgeonId,
        int? roomId,
        string? status)
    {
        var query =
            from utilization in _dbContext.UtilizationRecords
            join block in _dbContext.BlockAllocations
                on utilization.BlockId equals block.BlockId
            join surgeon in _dbContext.Surgeons
                on block.SurgeonId equals surgeon.SurgeonId
            join user in _dbContext.Users
                on surgeon.UserId equals user.UserId
            join room in _dbContext.OperatingRooms
                on block.ORRoomId equals room.ORRoomId
            where block.HospitalId == hospitalId
            select new
            {
                utilization,
                block,
                surgeon,
                user,
                room
            };

        if (fromDate.HasValue)
        {
            var from = DateOnly.FromDateTime(fromDate.Value.Date);
            query = query.Where(item => item.block.BlockDate >= from);
        }

        if (toDate.HasValue)
        {
            var to = DateOnly.FromDateTime(toDate.Value.Date);
            query = query.Where(item => item.block.BlockDate <= to);
        }

        if (surgeonId.HasValue)
        {
            query = query.Where(item => item.block.SurgeonId == surgeonId.Value);
        }

        if (roomId.HasValue)
        {
            query = query.Where(item => item.block.ORRoomId == roomId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(item => item.utilization.UtilStatus == status);
        }

        return await query
            .OrderByDescending(item => item.block.BlockDate)
            .Select(item => new UtilizationRecordDto
            {
                UtilizationId = item.utilization.UtilizationId,
                HospitalId = item.block.HospitalId,
                BlockId = item.utilization.BlockId,

                SurgeonId = item.block.SurgeonId,
                SurgeonName = item.user.FullName,

                ORRoomId = item.block.ORRoomId,
                RoomName = item.room.RoomName,

                BlockDate = item.block.BlockDate.ToDateTime(TimeOnly.MinValue),

                AllocatedMinutes = item.utilization.AllocatedMinutes,
                UsedMinutes = item.utilization.UsedMinutes,
                UtilizationPercent = item.utilization.UtilizationPct ?? 0,
                UtilizationStatus = item.utilization.UtilStatus,

                CalculatedAt = item.utilization.CalculatedAt
            })
            .ToListAsync();
    }
    public async Task<UtilizationSummaryDto> GetSummaryAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate)
    {
        var records = await GetUtilizationRecordsAsync(
            hospitalId,
            fromDate,
            toDate,
            null,
            null,
            null);

        if (records.Count == 0)
        {
            return new UtilizationSummaryDto();
        }

        return new UtilizationSummaryDto
        {
            TotalBlocks = records.Count,
            TotalAllocatedMinutes = records.Sum(record => record.AllocatedMinutes),
            TotalUsedMinutes = records.Sum(record => record.UsedMinutes),
            AverageUtilizationPercent = Math.Round(records.Average(record => record.UtilizationPercent), 2),

            GoodBlocks = records.Count(record => record.UtilizationStatus == "Good"),
            ModerateBlocks = records.Count(record => record.UtilizationStatus == "Moderate"),
            UnderutilizedBlocks = records.Count(record => record.UtilizationStatus == "Underutilized"),
            UnusedBlocks = records.Count(record => record.UtilizationStatus == "Unused")
        };
    }

    public async Task<List<UnderutilizedBlockDto>> GetUnderutilizedBlocksAsync(
        int hospitalId,
        DateTime? fromDate,
        DateTime? toDate)
    {
        var records = await GetUtilizationRecordsAsync(
            hospitalId,
            fromDate,
            toDate,
            null,
            null,
            null);

        return records
            .Where(record =>
                record.UtilizationStatus == "Underutilized" ||
                record.UtilizationStatus == "Unused")
            .Select(record => new UnderutilizedBlockDto
            {
                BlockId = record.BlockId,
                SurgeonId = record.SurgeonId,
                SurgeonName = record.SurgeonName,

                ORRoomId = record.ORRoomId,
                RoomName = record.RoomName,

                BlockDate = record.BlockDate,

                AllocatedMinutes = record.AllocatedMinutes,
                UsedMinutes = record.UsedMinutes,
                UtilizationPercent = record.UtilizationPercent,
                UtilizationStatus = record.UtilizationStatus
            })
            .OrderBy(record => record.UtilizationPercent)
            .ToList();
    }

    public async Task<bool> BlockExistsAsync(int hospitalId, int blockId)
    {
        return await _dbContext.BlockAllocations
            .AnyAsync(block =>
                block.HospitalId == hospitalId &&
                block.BlockId == blockId);
    }

    public async Task CalculateBlockUtilizationAsync(int blockId)
    {
        await _dbContext.Database.ExecuteSqlRawAsync(
            "EXEC analytics.usp_CalculateBlockUtilization @BlockId",
            new SqlParameter("@BlockId", blockId));
    }

    public async Task<List<int>> GetBlockIdsForDateRangeAsync(
        int hospitalId,
        DateTime fromDate,
        DateTime toDate)
    {
        var from = DateOnly.FromDateTime(fromDate.Date);
        var to = DateOnly.FromDateTime(toDate.Date);

        return await _dbContext.BlockAllocations
            .Where(block =>
                block.HospitalId == hospitalId &&
                block.BlockDate >= from &&
                block.BlockDate <= to &&
                block.BlockStatus != "Cancelled")
            .Select(block => block.BlockId)
            .ToListAsync();
    }
}