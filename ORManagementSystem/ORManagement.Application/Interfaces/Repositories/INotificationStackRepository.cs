using ORManagement.Application.DTOs.Notifications;

namespace ORManagement.Application.Interfaces.Repositories;

public interface INotificationStackRepository
{
    Task<List<NotificationStackItemDto>> GetAllAsync(int hospitalId, int take);

    Task<List<NotificationStackItemDto>> GetSentAsync(
        int hospitalId,
        int userId,
        int take);

    Task<List<NotificationStackItemDto>> GetReceivedAsync(
        int hospitalId,
        int userId,
        string roleName,
        int take);
}