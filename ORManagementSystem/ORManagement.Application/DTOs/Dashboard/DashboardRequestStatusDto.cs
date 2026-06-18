namespace ORManagement.Application.DTOs.Dashboard;

public class DashboardRequestStatusDto
{
    public int RequestId { get; set; }
    public string SurgeryType { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string PatientReadiness { get; set; } = string.Empty;
    public string RequestStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}