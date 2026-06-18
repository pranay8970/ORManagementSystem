using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Api.Controllers;

[Route("api/notification-stack")]
[Authorize]
public class NotificationStackController : ApiControllerBase
{
    private readonly INotificationStackService _notificationStackService;
    private readonly ILogger<NotificationStackController> _logger;

    public NotificationStackController(
        INotificationStackService notificationStackService,
        ILogger<NotificationStackController> logger)
    {
        _notificationStackService = notificationStackService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int take = 20)
    {
        var result = await _notificationStackService.GetAllAsync(
            GetCurrentHospitalIdOrDefault(),
            take);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpGet("received")]
    public async Task<IActionResult> GetReceived([FromQuery] int take = 20)
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

        var result = await _notificationStackService.GetReceivedAsync(
            GetCurrentHospitalIdOrDefault(),
            userId.Value,
            GetCurrentRoleName(),
            take);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpGet("sent")]
    public async Task<IActionResult> GetSent([FromQuery] int take = 20)
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

        var result = await _notificationStackService.GetSentAsync(
            GetCurrentHospitalIdOrDefault(),
            userId.Value,
            take);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }
}