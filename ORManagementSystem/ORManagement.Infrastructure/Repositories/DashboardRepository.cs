using Microsoft.EntityFrameworkCore;
using ORManagement.Application.DTOs.Dashboard;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Infrastructure.Data;

namespace ORManagement.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ORManagementDbContext _dbContext;

    public DashboardRepository(ORManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SurgeonDashboardDto> GetSurgeonDashboardAsync(
        int hospitalId,
        int surgeonId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var now = DateTime.UtcNow;
            
        var assignedBlocks = await _dbContext.BlockAllocations
            .CountAsync(block =>
                block.HospitalId == hospitalId &&
                block.SurgeonId == surgeonId &&
                block.BlockStatus == "Allocated");

        var upcomingBlocks = await _dbContext.BlockAllocations
    .CountAsync(block =>
        block.HospitalId == hospitalId &&
        block.SurgeonId == surgeonId &&
        block.BlockDate >= today &&
        block.BlockStatus != "Cancelled");

var pendingRequests = await _dbContext.ORRequests
    .CountAsync(request =>
        request.HospitalId == hospitalId &&
        request.SurgeonId == surgeonId &&
        request.RequestStatus == "PendingReview");

var approvedRequests = await _dbContext.ORRequests
    .CountAsync(request =>
        request.HospitalId == hospitalId &&
        request.SurgeonId == surgeonId &&
        request.RequestStatus == "Approved");

var waitlistedRequests = await _dbContext.ORRequests
    .CountAsync(request =>
        request.HospitalId == hospitalId &&
        request.SurgeonId == surgeonId &&
        request.RequestStatus == "Waitlisted");

var scheduledRequests = await _dbContext.ORRequests
    .CountAsync(request =>
        request.HospitalId == hospitalId &&
        request.SurgeonId == surgeonId &&
        request.RequestStatus == "Scheduled");

var upcomingSurgeries = await _dbContext.SurgicalCases
    .CountAsync(surgicalCase =>
        surgicalCase.HospitalId == hospitalId &&
        surgicalCase.SurgeonId == surgeonId &&
        surgicalCase.ScheduledStart >= now &&
        surgicalCase.CaseStatus != "Cancelled");

var completedSurgeries = await _dbContext.SurgicalCases
    .CountAsync(surgicalCase =>
        surgicalCase.HospitalId == hospitalId &&
        surgicalCase.SurgeonId == surgeonId &&
        surgicalCase.CaseStatus == "Completed");

var utilizationValues = await
    (
        from utilization in _dbContext.UtilizationRecords
        join block in _dbContext.BlockAllocations
            on utilization.BlockId equals block.BlockId
        where block.HospitalId == hospitalId &&
              block.SurgeonId == surgeonId
        select utilization.UtilizationPct ?? 0
    )
    .ToListAsync();

var averageUtilizationPercent = utilizationValues.Count == 0
    ? 0
    : Math.Round(utilizationValues.Average(), 2);

var upcomingCases = await
    (
        from surgicalCase in _dbContext.SurgicalCases
        join request in _dbContext.ORRequests
            on surgicalCase.RequestId equals request.RequestId
        join room in _dbContext.OperatingRooms
            on surgicalCase.ORRoomId equals room.ORRoomId
        where surgicalCase.HospitalId == hospitalId &&
              surgicalCase.SurgeonId == surgeonId &&
              surgicalCase.ScheduledStart >= now &&
              surgicalCase.CaseStatus != "Cancelled"
        orderby surgicalCase.ScheduledStart
        select new DashboardUpcomingCaseDto
        {
            SurgeryId = surgicalCase.SurgeryId,
            RequestId = surgicalCase.RequestId,
            RoomName = room.RoomName,
            SurgeryType = request.SurgeryType,
            ScheduledStart = surgicalCase.ScheduledStart,
            ScheduledEnd = surgicalCase.ScheduledEnd,
            CaseStatus = surgicalCase.CaseStatus
        }
    )
    .Take(5)
    .ToListAsync();

var recentRequests = await _dbContext.ORRequests
    .Where(request =>
        request.HospitalId == hospitalId &&
        request.SurgeonId == surgeonId)
    .OrderByDescending(request => request.CreatedAt)
    .Select(request => new DashboardRequestStatusDto
    {
        RequestId = request.RequestId,
        SurgeryType = request.SurgeryType,
        Priority = request.Priority,
        PatientReadiness = request.PatientReadiness,
        RequestStatus = request.RequestStatus,
        CreatedAt = request.CreatedAt
    })
    .Take(5)
    .ToListAsync();

return new SurgeonDashboardDto
{
    AssignedBlocks = assignedBlocks,
    UpcomingBlocks = upcomingBlocks,

    PendingRequests = pendingRequests,
    ApprovedRequests = approvedRequests,
    WaitlistedRequests = waitlistedRequests,
    ScheduledRequests = scheduledRequests,

    UpcomingSurgeries = upcomingSurgeries,
    CompletedSurgeries = completedSurgeries,

    AverageUtilizationPercent = averageUtilizationPercent,

    UpcomingCases = upcomingCases,
    RecentRequests = recentRequests
};
    }

    public async Task<SchedulerDashboardDto> GetSchedulerDashboardAsync(int hospitalId)
{
    var todayStart = DateTime.UtcNow.Date;
    var todayEnd = todayStart.AddDays(1);

    var activeRooms = await _dbContext.OperatingRooms
        .CountAsync(room =>
            room.HospitalId == hospitalId &&
            room.IsActive);

    var pendingRequests = await _dbContext.ORRequests
        .CountAsync(request =>
            request.HospitalId == hospitalId &&
            request.RequestStatus == "PendingReview");

    var approvedRequests = await _dbContext.ORRequests
        .CountAsync(request =>
            request.HospitalId == hospitalId &&
            request.RequestStatus == "Approved");

    var waitlistedRequests = await _dbContext.ORRequests
        .CountAsync(request =>
            request.HospitalId == hospitalId &&
            request.RequestStatus == "Waitlisted");

    var scheduledRequests = await _dbContext.ORRequests
        .CountAsync(request =>
            request.HospitalId == hospitalId &&
            request.RequestStatus == "Scheduled");

    var availableReleasedSlots = await _dbContext.ReleasedSlots
        .CountAsync(slot =>
            slot.HospitalId == hospitalId &&
            slot.SlotState == "Available");

    var assignedReleasedSlots = await _dbContext.ReleasedSlots
        .CountAsync(slot =>
            slot.HospitalId == hospitalId &&
            slot.SlotState == "Assigned");

    var totalWaitlistRequests = await
        (
            from waitlist in _dbContext.WaitlistRequests
            join request in _dbContext.ORRequests
                on waitlist.RequestId equals request.RequestId
            where request.HospitalId == hospitalId &&
                  request.RequestStatus == "Waitlisted"
            select waitlist.WaitlistId
        )
        .CountAsync();

    var starvedRequests = await _dbContext.ORRequests
        .CountAsync(request =>
            request.HospitalId == hospitalId &&
            request.RequestStatus == "Waitlisted" &&
            request.CyclesWaited >= 3);

    var todayScheduledCases = await _dbContext.SurgicalCases
        .CountAsync(surgicalCase =>
            surgicalCase.HospitalId == hospitalId &&
            surgicalCase.ScheduledStart >= todayStart &&
            surgicalCase.ScheduledStart < todayEnd &&
            surgicalCase.CaseStatus == "Scheduled");

    var todayInProgressCases = await _dbContext.SurgicalCases
        .CountAsync(surgicalCase =>
            surgicalCase.HospitalId == hospitalId &&
            surgicalCase.ScheduledStart >= todayStart &&
            surgicalCase.ScheduledStart < todayEnd &&
            surgicalCase.CaseStatus == "InProgress");

    var todayCompletedCases = await _dbContext.SurgicalCases
        .CountAsync(surgicalCase =>
            surgicalCase.HospitalId == hospitalId &&
            surgicalCase.ScheduledStart >= todayStart &&
            surgicalCase.ScheduledStart < todayEnd &&
            surgicalCase.CaseStatus == "Completed");

    var utilizationValues = await
        (
            from utilization in _dbContext.UtilizationRecords
            join block in _dbContext.BlockAllocations
                on utilization.BlockId equals block.BlockId
            where block.HospitalId == hospitalId
            select utilization.UtilizationPct ?? 0
        )
        .ToListAsync();

    var averageUtilizationPercent = utilizationValues.Count == 0
        ? 0
        : Math.Round(utilizationValues.Average(), 2);

    var todaySchedule = await GetTodayScheduleAsync(hospitalId, DateTime.UtcNow.Date);

    return new SchedulerDashboardDto
    {
        ActiveRooms = activeRooms,

        PendingRequests = pendingRequests,
        ApprovedRequests = approvedRequests,
        WaitlistedRequests = waitlistedRequests,
        ScheduledRequests = scheduledRequests,

        AvailableReleasedSlots = availableReleasedSlots,
        AssignedReleasedSlots = assignedReleasedSlots,

        TotalWaitlistRequests = totalWaitlistRequests,
        StarvedRequests = starvedRequests,

        TodayScheduledCases = todayScheduledCases,
        TodayInProgressCases = todayInProgressCases,
        TodayCompletedCases = todayCompletedCases,

        AverageUtilizationPercent = averageUtilizationPercent,

        TodaySchedule = todaySchedule
    };
}

public async Task<List<DashboardTodayScheduleDto>> GetTodayScheduleAsync(
    int hospitalId,
    DateTime date)
{
    var dayStart = date.Date;
    var dayEnd = dayStart.AddDays(1);

    return await
        (
            from surgicalCase in _dbContext.SurgicalCases
            join request in _dbContext.ORRequests
                on surgicalCase.RequestId equals request.RequestId
            join surgeon in _dbContext.Surgeons
                on surgicalCase.SurgeonId equals surgeon.SurgeonId
            join user in _dbContext.Users
                on surgeon.UserId equals user.UserId
            join room in _dbContext.OperatingRooms
                on surgicalCase.ORRoomId equals room.ORRoomId
            where surgicalCase.HospitalId == hospitalId &&
                  surgicalCase.ScheduledStart >= dayStart &&
                  surgicalCase.ScheduledStart < dayEnd
            orderby surgicalCase.ScheduledStart
            select new DashboardTodayScheduleDto
            {
                SurgeryId = surgicalCase.SurgeryId,
                RequestId = surgicalCase.RequestId,

                RoomName = room.RoomName,
                SurgeonName = user.FullName,
                SurgeryType = request.SurgeryType,

                ScheduledStart = surgicalCase.ScheduledStart,
                ScheduledEnd = surgicalCase.ScheduledEnd,

                CaseStatus = surgicalCase.CaseStatus
            }
        )
        .ToListAsync();
}
}


