using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ORManagement.Application.DTOs.Forecast;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Api.Controllers;

[Route("api/forecast")]
[Authorize(Roles = "ORScheduler")]
public class ForecastController : ApiControllerBase
{
    private readonly IForecastService _forecastService;
    private readonly ILogger<ForecastController> _logger;

    public ForecastController(
        IForecastService forecastService,
        ILogger<ForecastController> logger)
    {
        _forecastService = forecastService;
        _logger = logger;
    }

    [HttpGet("recommendations")]
    public async Task<IActionResult> GetRecommendations([FromQuery] string? status)
    {
        var result = await _forecastService.GetRecommendationsAsync(
            GetCurrentHospitalIdOrDefault(),
            status);

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpGet("demand")]
    public async Task<IActionResult> GetDemand()
    {
        var result = await _forecastService.GetDemandAsync(
            GetCurrentHospitalIdOrDefault());

        if (!result.Success)
        {
            return MapError(result);
        }

        return Ok(result.Data);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateRecommendations()
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

        var result = await _forecastService.GenerateRecommendationsAsync(
            GetCurrentHospitalIdOrDefault(),
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
            message = result.Message,
            generatedCount = result.Data
        });
    }

    [HttpPut("recommendations/{id:int}/status")]
    public async Task<IActionResult> UpdateRecommendationStatus(
        int id,
        [FromBody] UpdateForecastRecommendationStatusDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                errorCode = "VALIDATION_ERROR",
                message = "Invalid forecast status update request.",
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

        var result = await _forecastService.UpdateRecommendationStatusAsync(
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
}