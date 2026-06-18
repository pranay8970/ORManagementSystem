namespace ORManagement.Application.DTOs.Cases;

public class SurgicalCaseDto
{
    public int SurgeryId { get; set; }
    public int HospitalId { get; set; }

    public int RequestId { get; set; }
    public int BlockId { get; set; }
    public int SurgeonId { get; set; }
    public int ORRoomId { get; set; }

    public string SurgeonName { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;

    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientMrn { get; set; } = string.Empty;

    public string SurgeryType { get; set; } = string.Empty;

    public DateTime ScheduledStart { get; set; }
    public DateTime ScheduledEnd { get; set; }

    public DateTime? ActualStart { get; set; }
    public DateTime? ActualEnd { get; set; }

    public string CaseStatus { get; set; } = string.Empty;
    public string? CancellationReason { get; set; }
}