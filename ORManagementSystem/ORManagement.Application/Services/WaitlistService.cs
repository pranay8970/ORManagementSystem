using Microsoft.Extensions.Logging;
using ORManagement.Application.DTOs.Audit;
using ORManagement.Application.DTOs.Shared;
using ORManagement.Application.DTOs.Waitlist;
using ORManagement.Application.Engines;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Application.Services;

public class WaitlistService : IWaitlistService
{
    private readonly IWaitlistRepository _waitlistRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly AvailabilityWindowEngine _availabilityWindowEngine;
    private readonly ILogger<WaitlistService> _logger;

    private static readonly HashSet<string> AllowedSlotStates = new()
    {
        "Available",
        "Matched",
        "Assigned",
        "Expired"
    };

    public WaitlistService(
        IWaitlistRepository waitlistRepository,
        IAuditRepository auditRepository,
        AvailabilityWindowEngine availabilityWindowEngine,
        ILogger<WaitlistService> logger)
    {
        _waitlistRepository = waitlistRepository;
        _auditRepository = auditRepository;
        _availabilityWindowEngine = availabilityWindowEngine;
        _logger = logger;
    }

    public async Task<ServiceResultDto<List<ReleasedSlotDto>>> GetReleasedSlotsAsync(
        int hospitalId,
        string? state,
        DateTime? fromDate,
        DateTime? toDate)
    {
        var slots = await _waitlistRepository.GetReleasedSlotsAsync(
            hospitalId,
            state,
            fromDate,
            toDate);

        return ServiceResultDto<List<ReleasedSlotDto>>.Ok(slots);
    }

    public async Task<ServiceResultDto<List<WaitlistMatchDto>>> GetMatchesAsync(
        int hospitalId,
        int slotId)
    {
        var slot = await _waitlistRepository.GetReleasedSlotByIdAsync(hospitalId, slotId);

        if (slot is null)
        {
            return ServiceResultDto<List<WaitlistMatchDto>>.Fail(
                "RELEASED_SLOT_NOT_FOUND",
                "Released slot was not found.");
        }

        if (slot.SlotState != "Available")
        {
            return ServiceResultDto<List<WaitlistMatchDto>>.Fail(
                "INVALID_SLOT_STATE",
                "Only available slots can be matched.");
        }

        var matches = await _waitlistRepository.GetMatchesAsync(slotId);

        return ServiceResultDto<List<WaitlistMatchDto>>.Ok(matches);
    }

    public async Task<ServiceResultDto> UpdateReleasedSlotStatusAsync(
        int hospitalId,
        int slotId,
        int userId,
        string roleName,
        UpdateReleasedSlotStatusDto request,
        string? ipAddress,
        string? userAgent)
    {
        var slotState = request.SlotState.Trim();

        if (!AllowedSlotStates.Contains(slotState))
        {
            return ServiceResultDto.Fail("INVALID_SLOT_STATE", "Invalid slot state.");
        }

        var existingSlot = await _waitlistRepository.GetReleasedSlotByIdAsync(hospitalId, slotId);

        if (existingSlot is null)
        {
            return ServiceResultDto.Fail("RELEASED_SLOT_NOT_FOUND", "Released slot was not found.");
        }

        var updated = await _waitlistRepository.UpdateReleasedSlotStatusAsync(
            hospitalId,
            slotId,
            slotState);

        if (!updated)
        {
            return ServiceResultDto.Fail("SLOT_UPDATE_FAILED", "Released slot status could not be updated.");
        }

        await _auditRepository.AddAuditLogAsync(new CreateAuditLogDto
        {
            HospitalId = hospitalId,
            UserId = userId,
            RoleName = roleName,
            Action = "ReleasedSlotStatusUpdated",
            Entity = "ReleasedSlots",
            EntityId = slotId,
            OldValue = existingSlot.SlotState,
            NewValue = slotState,
            Remarks = "Released slot state updated.",
            IpAddress = ipAddress,
            UserAgent = userAgent
        });

        return ServiceResultDto.Ok("Released slot status updated successfully.");
    }

    public async Task<ServiceResultDto<List<WaitlistRequestDto>>> GetWaitlistAsync(int hospitalId)
    {
        var waitlist = await _waitlistRepository.GetWaitlistAsync(hospitalId);

        foreach (var item in waitlist)
        {
            item.AvailableDaysDisplay = _availabilityWindowEngine.ToDisplayText(item.AvailableDaysMask);
        }

        return ServiceResultDto<List<WaitlistRequestDto>>.Ok(waitlist);
    }

    public async Task<ServiceResultDto> AssignWaitlistAsync(
        int hospitalId,
        int waitlistId,
        int userId,
        string roleName,
        AssignWaitlistRequestDto request,
        string? ipAddress,
        string? userAgent)
    {
        var waitlist = await _waitlistRepository.GetWaitlistByIdAsync(hospitalId, waitlistId);

        if (waitlist is null)
        {
            return ServiceResultDto.Fail("WAITLIST_NOT_FOUND", "Waitlist request was not found.");
        }

        var slot = await _waitlistRepository.GetReleasedSlotByIdAsync(hospitalId, request.SlotId);

        if (slot is null)
        {
            return ServiceResultDto.Fail("RELEASED_SLOT_NOT_FOUND", "Released slot was not found.");
        }

        if (slot.SlotState != "Available" && slot.SlotState != "Matched")
        {
            return ServiceResultDto.Fail("INVALID_SLOT_STATE", "Only available or matched slots can be assigned.");
        }

        var assigned = await _waitlistRepository.AssignWaitlistAsync(
            hospitalId,
            waitlistId,
            request.SlotId,
            request.MatchScore,
            userId);

        if (!assigned)
        {
            return ServiceResultDto.Fail("WAITLIST_ASSIGN_FAILED", "Waitlist request could not be assigned.");
        }

        await _auditRepository.AddAuditLogAsync(new CreateAuditLogDto
        {
            HospitalId = hospitalId,
            UserId = userId,
            RoleName = roleName,
            Action = "WaitlistAssigned",
            Entity = "WaitlistRequests",
            EntityId = waitlistId,
            OldValue = "Waitlisted",
            NewValue = "Assigned",
            Remarks = $"Assigned to released slot {request.SlotId}.",
            IpAddress = ipAddress,
            UserAgent = userAgent
        });

        _logger.LogInformation(
            "Waitlist assigned. WaitlistId: {WaitlistId}, SlotId: {SlotId}, UserId: {UserId}",
            waitlistId,
            request.SlotId,
            userId);

        return ServiceResultDto.Ok("Waitlist request assigned successfully.");
    }

    public async Task<ServiceResultDto> RemoveWaitlistAsync(
        int hospitalId,
        int waitlistId,
        int userId,
        string roleName,
        string? ipAddress,
        string? userAgent)
    {
        var waitlist = await _waitlistRepository.GetWaitlistByIdAsync(hospitalId, waitlistId);

        if (waitlist is null)
        {
            return ServiceResultDto.Fail("WAITLIST_NOT_FOUND", "Waitlist request was not found.");
        }

        var removed = await _waitlistRepository.RemoveWaitlistAsync(hospitalId, waitlistId);

        if (!removed)
        {
            return ServiceResultDto.Fail("WAITLIST_REMOVE_FAILED", "Waitlist request could not be removed.");
        }

        await _auditRepository.AddAuditLogAsync(new CreateAuditLogDto
        {
            HospitalId = hospitalId,
            UserId = userId,
            RoleName = roleName,
            Action = "WaitlistRemoved",
            Entity = "WaitlistRequests",
            EntityId = waitlistId,
            OldValue = "Waitlisted",
            NewValue = "Modified",
            Remarks = "Waitlist request removed.",
            IpAddress = ipAddress,
            UserAgent = userAgent
        });

        return ServiceResultDto.Ok("Waitlist request removed successfully.");
    }

    public async Task<ServiceResultDto<List<WaitlistRequestDto>>> GetStarvedRequestsAsync(int hospitalId)
    {
        const int starvationThreshold = 3;

        var requests = await _waitlistRepository.GetStarvedRequestsAsync(
            hospitalId,
            starvationThreshold);

        foreach (var item in requests)
        {
            item.AvailableDaysDisplay = _availabilityWindowEngine.ToDisplayText(item.AvailableDaysMask);
        }

        return ServiceResultDto<List<WaitlistRequestDto>>.Ok(requests);
    }
}