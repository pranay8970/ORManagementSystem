using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Blocks;

public class UpdateBlockAllocationDto
{
    [Required]
    public int SurgeonId { get; set; }

    [Required]
    public int ORRoomId { get; set; }

    [Required]
    public DateTime BlockDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Required]
    public string BlockStatus { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Remarks { get; set; }
}