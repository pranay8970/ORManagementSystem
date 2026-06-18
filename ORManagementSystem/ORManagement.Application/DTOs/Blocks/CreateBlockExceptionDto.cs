using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Blocks;

public class CreateBlockExceptionDto
{
    [Required]
    public DateTime SkipDate { get; set; }

    [StringLength(200)]
    public string? Reason { get; set; }
}