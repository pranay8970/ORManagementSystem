using ORManagement.Application.DTOs.Waitlist;

namespace ORManagement.Application.Interfaces.Repositories;

public interface IWaitlistRepository
{
    Task<List<ReleasedSlotDto>> GetReleasedSlotsAsync(
        int hospitalId,
        string? state,
        DateTime? fromDate,
        DateTime? toDate);

    Task<ReleasedSlotDto?> GetReleasedSlotByIdAsync(int hospitalId, int slotId);

    Task<bool> UpdateReleasedSlotStatusAsync(
        int hospitalId,
        int slotId,
        string slotState);

    Task<List<WaitlistRequestDto>> GetWaitlistAsync(int hospitalId);
    Task<WaitlistRequestDto?> GetWaitlistByIdAsync(int hospitalId, int waitlistId);
    Task<List<WaitlistRequestDto>> GetStarvedRequestsAsync(int hospitalId, int starvationThreshold);

    Task<List<WaitlistMatchDto>> GetMatchesAsync(int slotId);

    Task<bool> AssignWaitlistAsync(
        int hospitalId,
        int waitlistId,
        int slotId,
        decimal? matchScore,
        int modifiedByUserId);

    Task<bool> RemoveWaitlistAsync(int hospitalId, int waitlistId);
}