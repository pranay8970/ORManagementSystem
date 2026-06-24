namespace ORManagement.Application.DTOs.Blocks;

public class BlockTemplateDto
{
    public int TemplateId { get; set; }

    public int? SurgeonId { get; set; }
    public string? SurgeonName { get; set; }

    public int ORRoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;

    public string? Specialty { get; set; }

    public int DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public string BlockType { get; set; } = "Recurring";
}