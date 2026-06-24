using Microsoft.Extensions.Logging;
using ORManagement.Application.DTOs.Audit;
using ORManagement.Application.DTOs.Settings;
using ORManagement.Application.DTOs.Shared;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Application.Services;

public class SettingsService : ISettingsService
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly ILogger<SettingsService> _logger;

    private static readonly HashSet<string> AllowedSettingKeys = new()
    {
        "ReleaseCutoffHours",
        "TurnoverMinutes",
        "EmergencyBufferQ1Min",
        "EmergencyBufferQ2Min",
        "RecurringWeeksAhead",
        "StarvationCycleThreshold",
        "DefaultWeeklySchedulingHours"
    };

    public SettingsService(
        ISettingsRepository settingsRepository,
        IAuditRepository auditRepository,
        ILogger<SettingsService> logger)
    {
        _settingsRepository = settingsRepository;
        _auditRepository = auditRepository;
        _logger = logger;
    }

    public async Task<ServiceResultDto<List<SystemSettingDto>>> GetSettingsAsync(int hospitalId)
    {
        var settings = await _settingsRepository.GetSettingsAsync(hospitalId);

        return ServiceResultDto<List<SystemSettingDto>>.Ok(settings);
    }

    public async Task<ServiceResultDto<SystemSettingDto>> GetSettingByKeyAsync(
        int hospitalId,
        string key)
    {
        var normalizedKey = key.Trim();

        var setting = await _settingsRepository.GetSettingByKeyAsync(
            hospitalId,
            normalizedKey);

        if (setting is null)
        {
            return ServiceResultDto<SystemSettingDto>.Fail(
                "SETTING_NOT_FOUND",
                "System setting was not found.");
        }

        return ServiceResultDto<SystemSettingDto>.Ok(setting);
    }

    public async Task<ServiceResultDto> UpdateSettingAsync(
        int hospitalId,
        int userId,
        string roleName,
        string key,
        UpdateSettingRequestDto request,
        string? ipAddress,
        string? userAgent)
    {
        var normalizedKey = key.Trim();
        var newValue = request.SettingValue.Trim();

        if (!AllowedSettingKeys.Contains(normalizedKey))
        {
            return ServiceResultDto.Fail(
                "INVALID_SETTING_KEY",
                "Invalid setting key.");
        }

        if (string.IsNullOrWhiteSpace(newValue))
        {
            return ServiceResultDto.Fail(
                "INVALID_SETTING_VALUE",
                "Setting value cannot be empty.");
        }

        var existingSetting = await _settingsRepository.GetSettingByKeyAsync(
            hospitalId,
            normalizedKey);

        if (existingSetting is null)
        {
            return ServiceResultDto.Fail(
                "SETTING_NOT_FOUND",
                "System setting was not found.");
        }

        var hospitalOverrideExists = await _settingsRepository.SettingExistsAsync(
            hospitalId,
            normalizedKey);

        if (hospitalOverrideExists)
        {
            var updated = await _settingsRepository.UpdateSettingAsync(
                hospitalId,
                normalizedKey,
                newValue);

            if (!updated)
            {
                return ServiceResultDto.Fail(
                    "SETTING_UPDATE_FAILED",
                    "System setting could not be updated.");
            }
        }
        else
        {
            await _settingsRepository.CreateHospitalSettingAsync(
                hospitalId,
                normalizedKey,
                newValue);
        }

        await _auditRepository.AddAuditLogAsync(new CreateAuditLogDto
        {
            HospitalId = hospitalId,
            UserId = userId,
            RoleName = roleName,
            Action = "SettingUpdated",
            Entity = "SystemSettings",
            EntityId = existingSetting.SettingId,
            OldValue = existingSetting.SettingValue,
            NewValue = newValue,
            Remarks = $"System setting '{normalizedKey}' updated.",
            IpAddress = ipAddress,
            UserAgent = userAgent
        });

        _logger.LogInformation(
            "System setting updated. Key: {SettingKey}, UserId: {UserId}, HospitalId: {HospitalId}",
            normalizedKey,
            userId,
            hospitalId);

        return ServiceResultDto.Ok("System setting updated successfully.");
    }
}