namespace ORManagement.Application.DTOs.Waitlist;

public class ReleasedSlotDto
{
    public int SlotId { get; set; }
    public int HospitalId { get; set; }
    public int BlockId { get; set; }
    public int ORRoomId { get; set; }

    public string RoomName { get; set; } = string.Empty;

    public DateTime SlotDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public string Source { get; set; } = string.Empty;
    public int? ReleasedByUserId { get; set; }
    public string SlotState { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}