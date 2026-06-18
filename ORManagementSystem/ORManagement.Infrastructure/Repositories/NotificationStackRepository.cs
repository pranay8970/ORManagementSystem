using Microsoft.EntityFrameworkCore;
using ORManagement.Application.DTOs.Notifications;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Infrastructure.Data;

namespace ORManagement.Infrastructure.Repositories;

public class NotificationStackRepository : INotificationStackRepository
{
    private readonly ORManagementDbContext _dbContext;

    public NotificationStackRepository(ORManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<NotificationStackItemDto>> GetAllAsync(int hospitalId, int take)
    {
        return await _dbContext.AuditLogs
            .Where(log => log.HospitalId == hospitalId)
            .OrderByDescending(log => log.CreatedAt)
            .Take(take)
            .Select(log => new NotificationStackItemDto
            {
                AuditId = log.AuditId,
                HospitalId = log.HospitalId,
                UserId = log.UserId,
                RoleName = log.RoleName,
                Action = log.Action,
                Entity = log.Entity,
                EntityId = log.EntityId,
                Message = BuildMessage(log.Action, log.Entity, log.EntityId),
                Remarks = log.Remarks,
                CreatedAt = log.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<NotificationStackItemDto>> GetSentAsync(
        int hospitalId,
        int userId,
        int take)
    {
        return await _dbContext.AuditLogs
            .Where(log =>
                log.HospitalId == hospitalId &&
                log.UserId == userId)
            .OrderByDescending(log => log.CreatedAt)
            .Take(take)
            .Select(log => new NotificationStackItemDto
            {
                AuditId = log.AuditId,
                HospitalId = log.HospitalId,
                UserId = log.UserId,
                RoleName = log.RoleName,
                Action = log.Action,
                Entity = log.Entity,
                EntityId = log.EntityId,
                Message = BuildMessage(log.Action, log.Entity, log.EntityId),
                Remarks = log.Remarks,
                CreatedAt = log.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<NotificationStackItemDto>> GetReceivedAsync(
        int hospitalId,
        int userId,
        string roleName,
        int take)
    {
        var receivedActions = roleName == "Surgeon"
            ? new[]
            {
                "RequestApproved",
                "RequestRejected",
                "RequestModified",
                "RequestWaitlisted",
                "RequestScheduled",
                "CaseScheduled",
                "CaseUpdated",
                "CaseCancelled"
            }
            : new[]
            {
                "RequestSubmitted",
                "BlockReleased",
                "WaitlistAssigned",
                "ForecastRecommendationsGenerated",
                "UtilizationCalculated"
            };

        return await _dbContext.AuditLogs
            .Where(log =>
                log.HospitalId == hospitalId &&
                log.UserId != userId &&
                receivedActions.Contains(log.Action))
            .OrderByDescending(log => log.CreatedAt)
            .Take(take)
            .Select(log => new NotificationStackItemDto
            {
                AuditId = log.AuditId,
                HospitalId = log.HospitalId,
                UserId = log.UserId,
                RoleName = log.RoleName,
                Action = log.Action,
                Entity = log.Entity,
                EntityId = log.EntityId,
                Message = BuildMessage(log.Action, log.Entity, log.EntityId),
                Remarks = log.Remarks,
                CreatedAt = log.CreatedAt
            })
            .ToListAsync();
    }

    private static string BuildMessage(string action, string entity, int? entityId)
    {
        var entityText = entityId.HasValue
            ? $"{entity} #{entityId.Value}"
            : entity;

        return action switch
        {
            "RequestSubmitted" => $"New OR request submitted: {entityText}.",
            "RequestApproved" => $"OR request approved: {entityText}.",
            "RequestRejected" => $"OR request rejected: {entityText}.",
            "RequestModified" => $"OR request marked for modification: {entityText}.",
            "RequestWaitlisted" => $"OR request waitlisted: {entityText}.",
            "RequestScheduled" => $"OR request scheduled: {entityText}.",
            "CaseScheduled" => $"Surgical case scheduled: {entityText}.",
            "CaseCompleted" => $"Surgical case completed: {entityText}.",
            "BlockReleased" => $"OR block time released: {entityText}.",
            "WaitlistAssigned" => $"Waitlisted request assigned: {entityText}.",
            "UtilizationCalculated" => $"Utilization calculated: {entityText}.",
            "ForecastRecommendationsGenerated" => "Forecast recommendations generated.",
            _ => $"{action}: {entityText}."
        };
    }
}