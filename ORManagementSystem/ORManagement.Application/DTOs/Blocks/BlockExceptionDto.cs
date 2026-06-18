namespace ORManagement.Application.DTOs.Blocks;

public class BlockExceptionDto
{
    public int ExceptionId { get; set; }
    public int TemplateId { get; set; }
    public DateTime SkipDate { get; set; }
    public string? Reason { get; set; }
}