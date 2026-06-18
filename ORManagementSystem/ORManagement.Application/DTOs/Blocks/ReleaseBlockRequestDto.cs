using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Blocks;

public class ReleaseBlockRequestDto
{
    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [StringLength(300)]
    public string? Remarks { get; set; }
}