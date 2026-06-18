namespace ORManagement.Application.DTOs.Waitlist;

public class WaitlistMatchDto
{
    public int WaitlistId { get; set; }
    public int RequestId { get; set; }
    public int SurgeonId { get; set; }

    public string SurgeryType { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string PatientReadiness { get; set; } = string.Empty;

    public int EstimatedDurationMin { get; set; }
    public int CyclesWaited { get; set; }
    public int WaitingDays { get; set; }

    public int SlotMin { get; set; }
    public decimal MatchScore { get; set; }
}