using Microsoft.EntityFrameworkCore;
using ORManagement.Application.DTOs.Settings;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Infrastructure.Data;
using ORManagement.Infrastructure.Data.Entities;

namespace ORManagement.Infrastructure.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly ORManagementDbContext _dbContext;

    public SettingsRepository(ORManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SystemSettingDto>> GetSettingsAsync(int hospitalId)
    {
        var settings = await _dbContext.SystemSettings
            .Where(setting =>
                setting.HospitalId == null ||
                setting.HospitalId == hospitalId)
            .OrderBy(setting => setting.SettingKey)
            .Select(setting => new SystemSettingDto
            {
                SettingId = setting.SettingId,
                HospitalId = setting.HospitalId,
                SettingKey = setting.SettingKey,
                SettingValue = setting.SettingValue
            })
            .ToListAsync();

        return settings
            .GroupBy(setting => setting.SettingKey)
            .Select(group =>
                group.FirstOrDefault(setting => setting.HospitalId == hospitalId)
                ?? group.First())
            .ToList();
    }

    public async Task<SystemSettingDto?> GetSettingByKeyAsync(
        int hospitalId,
        string key)
    {
        var settings = await _dbContext.SystemSettings
            .Where(setting =>
                setting.SettingKey == key &&
                (setting.HospitalId == null || setting.HospitalId == hospitalId))
            .OrderByDescending(setting => setting.HospitalId == hospitalId)
            .Select(setting => new SystemSettingDto
            {
                SettingId = setting.SettingId,
                HospitalId = setting.HospitalId,
                SettingKey = setting.SettingKey,
                SettingValue = setting.SettingValue
            })
            .ToListAsync();

        return settings.FirstOrDefault();
    }

    public async Task<bool> SettingExistsAsync(
        int hospitalId,
        string key)
    {
        return await _dbContext.SystemSettings
            .AnyAsync(setting =>
                setting.SettingKey == key &&
                setting.HospitalId == hospitalId);
    }

    public async Task<bool> UpdateSettingAsync(
        int hospitalId,
        string key,
        string value)
    {
        var setting = await _dbContext.SystemSettings
            .FirstOrDefaultAsync(setting =>
                setting.HospitalId == hospitalId &&
                setting.SettingKey == key);

        if (setting is null)
        {
            return false;
        }

        setting.SettingValue = value;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<int> CreateHospitalSettingAsync(
        int hospitalId,
        string key,
        string value)
    {
        var setting = new SystemSetting
        {
            HospitalId = hospitalId,
            SettingKey = key,
            SettingValue = value
        };

        await _dbContext.SystemSettings.AddAsync(setting);
        await _dbContext.SaveChangesAsync();

        return setting.SettingId;
    }
}