using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Cases;

public class UpdateCaseRequestDto
{
    [Required]
    public DateTime ScheduledStart { get; set; }

    [Required]
    public DateTime ScheduledEnd { get; set; }
}
