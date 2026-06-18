namespace ORManagement.Application.DTOs.Dashboard;

public class SchedulerDashboardDto
{
    public int ActiveRooms { get; set; }

    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int WaitlistedRequests { get; set; }
    public int ScheduledRequests { get; set; }

    public int AvailableReleasedSlots { get; set; }
    public int AssignedReleasedSlots { get; set; }

    public int TotalWaitlistRequests { get; set; }
    public int StarvedRequests { get; set; }

    public int TodayScheduledCases { get; set; }
    public int TodayInProgressCases { get; set; }
    public int TodayCompletedCases { get; set; }

    public decimal AverageUtilizationPercent { get; set; }

    public List<DashboardTodayScheduleDto> TodaySchedule { get; set; } = new();
}