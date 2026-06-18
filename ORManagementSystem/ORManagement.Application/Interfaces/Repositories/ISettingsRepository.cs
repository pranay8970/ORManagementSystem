using ORManagement.Application.DTOs.Settings;

namespace ORManagement.Application.Interfaces.Repositories;

public interface ISettingsRepository
{
    Task<List<SystemSettingDto>> GetSettingsAsync(int hospitalId);

    Task<SystemSettingDto?> GetSettingByKeyAsync(
        int hospitalId,
        string key);

    Task<bool> SettingExistsAsync(
        int hospitalId,
        string key);

    Task<bool> UpdateSettingAsync(
        int hospitalId,
        string key,
        string value);

    Task<int> CreateHospitalSettingAsync(
        int hospitalId,
        string key,
        string value);
}