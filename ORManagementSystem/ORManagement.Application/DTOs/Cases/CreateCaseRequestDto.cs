using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Cases;

public class CreateCaseRequestDto
{
    [Required]
    public int RequestId { get; set; }

    [Required]
    public int BlockId { get; set; }

    [Required]
    public DateTime ScheduledStart { get; set; }

    [Required]
    public DateTime ScheduledEnd { get; set; }
}