using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Api.Controllers;

[Route("api/dashboard")]
[Authorize]
public class DashboardController : ApiControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService dashboardService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet("surgeon")]
    [Authorize(Roles = "Surgeon")]
    public async Task<IActionResult> GetSurgeonDashboard()
    {
        var surgeonId = GetCurrentSurgeonId();

        if (surgeonId is null)
        {
            return Unauthorized(new
            {
                success = false,
                errorCode = "SURGEON_CLAIM_MISSING",
                message = "Surgeon profile was not found in token."
            });
        }

        var result = await _dashboardService.GetSurgeonDashboardAsync(
            GetCurrentHospitalIdOrDefault(),
            surgeonId.Value);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpGet("scheduler")]
    [Authorize(Roles = "ORScheduler")]
    public async Task<IActionResult> GetSchedulerDashboard()
    {
        var result = await _dashboardService.GetSchedulerDashboardAsync(
            GetCurrentHospitalIdOrDefault());

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpGet("today")]
    [Authorize(Roles = "ORScheduler")]
    public async Task<IActionResult> GetTodaySchedule([FromQuery] DateTime? date)
    {
        var result = await _dashboardService.GetTodayScheduleAsync(
            GetCurrentHospitalIdOrDefault(),
            date);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }
}



