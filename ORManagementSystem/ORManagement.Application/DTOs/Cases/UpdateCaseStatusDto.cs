using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Cases;

public class UpdateCaseStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;

    public DateTime? ActualStart { get; set; }
    public DateTime? ActualEnd { get; set; }

    public string? CancellationReason { get; set; }
}