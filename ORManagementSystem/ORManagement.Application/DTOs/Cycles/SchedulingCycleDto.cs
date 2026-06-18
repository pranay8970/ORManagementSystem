namespace ORManagement.Application.DTOs.Cycles;

public class SchedulingCycleDto
{
    public int CycleId { get; set; }
    public int HospitalId { get; set; }

    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public DateTime CutoffAt { get; set; }

    public string CycleStatus { get; set; } = string.Empty;
}