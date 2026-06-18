namespace ORManagement.Application.DTOs.Utilization;

public class UtilizationSummaryDto
{
    public int TotalBlocks { get; set; }
    public int TotalAllocatedMinutes { get; set; }
    public int TotalUsedMinutes { get; set; }

    public decimal AverageUtilizationPercent { get; set; }

    public int GoodBlocks { get; set; }
    public int ModerateBlocks { get; set; }
    public int UnderutilizedBlocks { get; set; }
    public int UnusedBlocks { get; set; }
}