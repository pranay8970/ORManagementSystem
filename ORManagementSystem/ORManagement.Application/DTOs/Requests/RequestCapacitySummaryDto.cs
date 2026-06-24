namespace ORManagement.Application.DTOs.Requests;

public class RequestCapacitySummaryDto
{
    public decimal SchedulingHourCapacity { get; set; }

    public decimal AllocatedHourCapacity { get; set; }

    public decimal RemainingHourCapacity { get; set; }

    public List<TopDoctorRecurringCapacityDto> TopRecurringDoctors { get; set; } = new();
}

public class TopDoctorRecurringCapacityDto
{
    public int SurgeonId { get; set; }

    public string SurgeonName { get; set; } = string.Empty;

    public decimal RecurringHours { get; set; }
}