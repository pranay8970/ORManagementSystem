namespace ORManagement.Application.DTOs.Audit;

public class AuditLogDto
{
    public int AuditId { get; set; }
    public int? HospitalId { get; set; }
    public int? UserId { get; set; }

    public string RoleName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;

    public int? EntityId { get; set; }

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Remarks { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }
}