using ORManagement.Application.DTOs.Shared;
using ORManagement.Application.DTOs.Waitlist;

namespace ORManagement.Application.Interfaces.Services;

public interface IWaitlistService
{
    Task<ServiceResultDto<List<ReleasedSlotDto>>> GetReleasedSlotsAsync(
        int hospitalId,
        string? state,
        DateTime? fromDate,
        DateTime? toDate);

    Task<ServiceResultDto<List<WaitlistMatchDto>>> GetMatchesAsync(
        int hospitalId,
        int slotId);

    Task<ServiceResultDto> UpdateReleasedSlotStatusAsync(
        int hospitalId,
        int slotId,
        int userId,
        string roleName,
        UpdateReleasedSlotStatusDto request,
        string? ipAddress,
        string? userAgent);

    Task<ServiceResultDto<List<WaitlistRequestDto>>> GetWaitlistAsync(int hospitalId);

    Task<ServiceResultDto> AssignWaitlistAsync(
        int hospitalId,
        int waitlistId,
        int userId,
        string roleName,
        AssignWaitlistRequestDto request,
        string? ipAddress,
        string? userAgent);

    Task<ServiceResultDto> RemoveWaitlistAsync(
        int hospitalId,
        int waitlistId,
        int userId,
        string roleName,
        string? ipAddress,
        string? userAgent);

    Task<ServiceResultDto<List<WaitlistRequestDto>>> GetStarvedRequestsAsync(int hospitalId);
}