using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORManagement.Application.DTOs.Waitlist;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Api.Controllers;

[Route("api")]
[Authorize(Roles = "ORScheduler")]
public class WaitlistController : ApiControllerBase
{
    private readonly IWaitlistService _waitlistService;
    private readonly ILogger<WaitlistController> _logger;

    public WaitlistController(
        IWaitlistService waitlistService,
        ILogger<WaitlistController> logger)
    {
        _waitlistService = waitlistService;
        _logger = logger;
    }

    [HttpGet("released-slots")]
    public async Task<IActionResult> GetReleasedSlots(
        [FromQuery] string? state,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var result = await _waitlistService.GetReleasedSlotsAsync(
            GetCurrentHospitalIdOrDefault(),
            state,
            fromDate,
            toDate);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpGet("released-slots/{id:int}/matches")]
    public async Task<IActionResult> GetMatches(int id)
    {
        var result = await _waitlistService.GetMatchesAsync(
            GetCurrentHospitalIdOrDefault(),
            id);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpPut("released-slots/{id:int}/status")]
    public async Task<IActionResult> UpdateReleasedSlotStatus(
        int id,
        [FromBody] UpdateReleasedSlotStatusDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errorCode = "VALIDATION_ERROR",
                message = "Invalid released slot status request.",
                errors = ModelState
            });
        }

        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                success = false,
                errorCode = "INVALID_TOKEN",
                message = "Invalid token."
            });
        }

        var result = await _waitlistService.UpdateReleasedSlotStatusAsync(
            GetCurrentHospitalIdOrDefault(),
            id,
            userId.Value,
            GetCurrentRoleName(),
            request,
            GetIpAddress(),
            GetUserAgent());

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(new
        {
            success = true,
            message = result.Message
        });
    }

    [HttpGet("waitlist")]
    public async Task<IActionResult> GetWaitlist()
    {
        var result = await _waitlistService.GetWaitlistAsync(
            GetCurrentHospitalIdOrDefault());

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpPut("waitlist/{id:int}/assign")]
    public async Task<IActionResult> AssignWaitlist(
        int id,
        [FromBody] AssignWaitlistRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errorCode = "VALIDATION_ERROR",
                message = "Invalid waitlist assignment request.",
                errors = ModelState
            });
        }

        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                success = false,
                errorCode = "INVALID_TOKEN",
                message = "Invalid token."
            });
        }

        var result = await _waitlistService.AssignWaitlistAsync(
            GetCurrentHospitalIdOrDefault(),
            id,
            userId.Value,
            GetCurrentRoleName(),
            request,
            GetIpAddress(),
            GetUserAgent());

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(new
        {
            success = true,
            message = result.Message
        });
    }

    [HttpDelete("waitlist/{id:int}")]
    public async Task<IActionResult> RemoveWaitlist(int id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
        {
            return Unauthorized(new
            {
                success = false,
                errorCode = "INVALID_TOKEN",
                message = "Invalid token."
            });
        }

        var result = await _waitlistService.RemoveWaitlistAsync(
            GetCurrentHospitalIdOrDefault(),
            id,
            userId.Value,
            GetCurrentRoleName(),
            GetIpAddress(),
            GetUserAgent());

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(new
        {
            success = true,
            message = result.Message
        });
    }

    [HttpGet("waitlist/starvation")]
    public async Task<IActionResult> GetStarvedRequests()
    {
        var result = await _waitlistService.GetStarvedRequestsAsync(
            GetCurrentHospitalIdOrDefault());

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }
}