using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Blocks;

public class UpdateBlockTemplateDto
{
    [Required]
    public int SurgeonId { get; set; }

    [Required]
    public int ORRoomId { get; set; }

    [Required]
    [StringLength(100)]
    public string Specialty { get; set; } = string.Empty;

    [Range(1, 7)]
    public byte DayOfWeek { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Required]
    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;
}