using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Blocks;

public class CreateBlockTemplateDto
{
    public int? SurgeonId { get; set; }

    [Required]
    public int ORRoomId { get; set; }

    public string? Specialty { get; set; }

    [Required]
    public int DayOfWeek { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Required]
    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public string BlockType { get; set; } = "Recurring";
}