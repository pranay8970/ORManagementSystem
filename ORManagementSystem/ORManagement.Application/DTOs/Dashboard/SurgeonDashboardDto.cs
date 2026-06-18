namespace ORManagement.Application.DTOs.Dashboard;

public class SurgeonDashboardDto
{
    public int AssignedBlocks { get; set; }
    public int UpcomingBlocks { get; set; }

    public int PendingRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int WaitlistedRequests { get; set; }
    public int ScheduledRequests { get; set; }

    public int UpcomingSurgeries { get; set; }
    public int CompletedSurgeries { get; set; }

    public decimal AverageUtilizationPercent { get; set; }

    public List<DashboardUpcomingCaseDto> UpcomingCases { get; set; } = new();
    public List<DashboardRequestStatusDto> RecentRequests { get; set; } = new();
}