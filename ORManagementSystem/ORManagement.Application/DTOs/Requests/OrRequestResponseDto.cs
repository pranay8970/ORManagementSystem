namespace ORManagement.Application.DTOs.Requests;

public class OrRequestResponseDto
{
    public int RequestId { get; set; }
    public int HospitalId { get; set; }
    public int SurgeonId { get; set; }
    public int PatientId { get; set; }

    public string SurgeonName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string PatientMrn { get; set; } = string.Empty;

    public int? CycleId { get; set; }
    public int? OriginalCycleId { get; set; }
    public int CyclesWaited { get; set; }

    public DateTime PreferredDate { get; set; }
    public string PreferredQuarter { get; set; } = string.Empty;
    public int EstimatedDurationMin { get; set; }

    public string SurgeryType { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string PatientReadiness { get; set; } = string.Empty;

    public string RequestStatus { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? SchedulerRemarks { get; set; }

    public int AvailableDaysMask { get; set; }
    public string AvailableDaysDisplay { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}