namespace ORManagement.Application.DTOs.Blocks;

public class BlockTemplateGenerationDto
{
    public int TemplateId { get; set; }
    public int SurgeonId { get; set; }
    public int RoomId { get; set; }

    public string Specialty { get; set; } = string.Empty;

    public byte DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}