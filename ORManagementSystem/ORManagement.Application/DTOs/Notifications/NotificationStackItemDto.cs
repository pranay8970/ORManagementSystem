namespace ORManagement.Application.DTOs.Notifications;

public class NotificationStackItemDto
{
    public int AuditId { get; set; }
    public int? HospitalId { get; set; }
    public int? UserId { get; set; }

    public string RoleName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;

    public int? EntityId { get; set; }

    public string? Message { get; set; }
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }
}