using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Waitlist;

public class UpdateReleasedSlotStatusDto
{
    [Required]
    public string SlotState { get; set; } = string.Empty;
}