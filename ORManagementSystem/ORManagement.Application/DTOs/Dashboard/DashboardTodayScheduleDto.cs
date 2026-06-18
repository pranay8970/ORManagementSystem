namespace ORManagement.Application.DTOs.Dashboard;

public class DashboardTodayScheduleDto
{
    public int SurgeryId { get; set; }
    public int RequestId { get; set; }

    public string RoomName { get; set; } = string.Empty;
    public string SurgeonName { get; set; } = string.Empty;
    public string SurgeryType { get; set; } = string.Empty;

    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }

    public string CaseStatus { get; set; } = string.Empty;
}