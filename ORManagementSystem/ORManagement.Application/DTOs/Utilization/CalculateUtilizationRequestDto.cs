using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Utilization;

public class CalculateUtilizationRequestDto
{
    public int? BlockId { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
}