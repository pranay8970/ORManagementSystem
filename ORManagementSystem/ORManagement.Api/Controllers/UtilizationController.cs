using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORManagement.Application.DTOs.Utilization;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Api.Controllers;

[Route("api/utilization")]
[Authorize(Roles = "ORScheduler")]
public class UtilizationController : ApiControllerBase
{
    private readonly IUtilizationService _utilizationService;
    private readonly ILogger<UtilizationController> _logger;

    public UtilizationController(
        IUtilizationService utilizationService,
        ILogger<UtilizationController> logger)
    {
        _utilizationService = utilizationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUtilizationRecords(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? surgeonId,
        [FromQuery] int? roomId,
        [FromQuery] string? status)
    {
        var result = await _utilizationService.GetUtilizationRecordsAsync(
            GetCurrentHospitalIdOrDefault(),
            fromDate,
            toDate,
            surgeonId,
            roomId,
            status);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var result = await _utilizationService.GetSummaryAsync(
            GetCurrentHospitalIdOrDefault(),
            fromDate,
            toDate);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpGet("underutilized")]
    public async Task<IActionResult> GetUnderutilizedBlocks(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var result = await _utilizationService.GetUnderutilizedBlocksAsync(
            GetCurrentHospitalIdOrDefault(),
            fromDate,
            toDate);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateUtilization([FromBody] CalculateUtilizationRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errorCode = "VALIDATION_ERROR",
                message = "Invalid utilization calculation request.",
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

        var result = await _utilizationService.CalculateUtilizationAsync(
            GetCurrentHospitalIdOrDefault(),
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
            message = result.Message,
            calculatedCount = result.Data
        });
    }
}