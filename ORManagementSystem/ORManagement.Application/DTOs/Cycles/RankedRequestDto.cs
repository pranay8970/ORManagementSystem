namespace ORManagement.Application.DTOs.Cycles;

public class RankedRequestDto
{
    public int RequestId { get; set; }
    public int SurgeonId { get; set; }
    public int PatientId { get; set; }

    public string SurgeryType { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string PatientReadiness { get; set; } = string.Empty;

    public int EstimatedDurationMin { get; set; }
    public string PreferredQuarter { get; set; } = string.Empty;

    public int AvailableDaysMask { get; set; }
    public string AvailableDaysDisplay { get; set; } = string.Empty;

    public int CyclesWaited { get; set; }
    public int WaitingDays { get; set; }

    public decimal RankScore { get; set; }
    public bool IsStarved { get; set; }
}