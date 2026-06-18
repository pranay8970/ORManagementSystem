using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Blocks;

public class GenerateBlocksRequestDto
{
    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }
}