using ORManagement.Application.DTOs.Settings;
using ORManagement.Application.DTOs.Shared;

namespace ORManagement.Application.Interfaces.Services;

public interface ISettingsService
{
    Task<ServiceResultDto<List<SystemSettingDto>>> GetSettingsAsync(int hospitalId);

    Task<ServiceResultDto<SystemSettingDto>> GetSettingByKeyAsync(
        int hospitalId,
        string key);

    Task<ServiceResultDto> UpdateSettingAsync(
        int hospitalId,
        int userId,
        string roleName,
        string key,
        UpdateSettingRequestDto request,
        string? ipAddress,
        string? userAgent);
}