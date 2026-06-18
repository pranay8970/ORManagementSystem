using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Waitlist;

public class AssignWaitlistRequestDto
{
    [Required]
    public int SlotId { get; set; }

    public decimal? MatchScore { get; set; }
}