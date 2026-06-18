using ORManagement.Application.DTOs.Notifications;
using ORManagement.Application.DTOs.Shared;

namespace ORManagement.Application.Interfaces.Services;

public interface INotificationStackService
{
    Task<ServiceResultDto<List<NotificationStackItemDto>>> GetAllAsync(int hospitalId, int take);

    Task<ServiceResultDto<List<NotificationStackItemDto>>> GetSentAsync(
        int hospitalId,
        int userId,
        int take);

    Task<ServiceResultDto<List<NotificationStackItemDto>>> GetReceivedAsync(
        int hospitalId,
        int userId,
        string roleName,
        int take);
}