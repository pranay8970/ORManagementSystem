using System.ComponentModel.DataAnnotations;

namespace ORManagement.Application.DTOs.Settings;

public class UpdateSettingRequestDto
{
    [Required]
    [StringLength(200)]
    public string SettingValue { get; set; } = string.Empty;
}