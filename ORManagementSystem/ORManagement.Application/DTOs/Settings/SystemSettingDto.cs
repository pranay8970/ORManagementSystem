namespace ORManagement.Application.DTOs.Settings;

public class SystemSettingDto
{
    public int SettingId { get; set; }
    public int? HospitalId { get; set; }

    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
}