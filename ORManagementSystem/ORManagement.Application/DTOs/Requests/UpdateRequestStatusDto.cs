using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Requests;

public class UpdateRequestStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;

    [StringLength(300)]
    public string? SchedulerRemarks { get; set; }
}