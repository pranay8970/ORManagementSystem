using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Requests;

public class UpdateOrRequestDto
{
    [Required]
    public DateTime PreferredDate { get; set; }

    [Required]
    public string PreferredQuarter { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int EstimatedDurationMin { get; set; }

    [Required]
    [StringLength(100)]
    public string SurgeryType { get; set; } = string.Empty;

    [Required]
    public string Priority { get; set; } = string.Empty;

    [Required]
    public string PatientReadiness { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Remarks { get; set; }

    public int AvailableDaysMask { get; set; } = 31;
}