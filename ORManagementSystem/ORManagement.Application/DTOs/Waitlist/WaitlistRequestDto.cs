namespace ORManagement.Application.DTOs.Waitlist;

public class WaitlistRequestDto
{
    public int WaitlistId { get; set; }
    public int RequestId { get; set; }

    public int SurgeonId { get; set; }
    public string SurgeonName { get; set; } = string.Empty;

    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientMrn { get; set; } = string.Empty;

    public string SurgeryType { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string PatientReadiness { get; set; } = string.Empty;

    public int EstimatedDurationMin { get; set; }
    public int CyclesWaited { get; set; }

    public int AvailableDaysMask { get; set; }
    public string AvailableDaysDisplay { get; set; } = string.Empty;

    public DateTime WaitingSince { get; set; }
    public decimal? MatchScore { get; set; }

    public int? MatchedSlotId { get; set; }
}