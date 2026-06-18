using Microsoft.Extensions.Logging;
using ORManagement.Application.DTOs.Notifications;
using ORManagement.Application.DTOs.Shared;
using ORManagement.Application.Interfaces.Repositories;
using ORManagement.Application.Interfaces.Services;

namespace ORManagement.Application.Services;

public class NotificationStackService : INotificationStackService
{
    private readonly INotificationStackRepository _notificationStackRepository;
    private readonly ILogger<NotificationStackService> _logger;

    public NotificationStackService(
        INotificationStackRepository notificationStackRepository,
        ILogger<NotificationStackService> logger)
    {
        _notificationStackRepository = notificationStackRepository;
        _logger = logger;
    }

    public async Task<ServiceResultDto<List<NotificationStackItemDto>>> GetAllAsync(
        int hospitalId,
        int take)
    {
        var safeTake = NormalizeTake(take);

        var items = await _notificationStackRepository.GetAllAsync(
            hospitalId,
            safeTake);

        return ServiceResultDto<List<NotificationStackItemDto>>.Ok(items);
    }

    public async Task<ServiceResultDto<List<NotificationStackItemDto>>> GetSentAsync(
        int hospitalId,
        int userId,
        int take)
    {
        var safeTake = NormalizeTake(take);

        var items = await _notificationStackRepository.GetSentAsync(
            hospitalId,
            userId,
            safeTake);

        return ServiceResultDto<List<NotificationStackItemDto>>.Ok(items);
    }

    public async Task<ServiceResultDto<List<NotificationStackItemDto>>> GetReceivedAsync(
        int hospitalId,
        int userId,
        string roleName,
        int take)
    {
        var safeTake = NormalizeTake(take);

        var items = await _notificationStackRepository.GetReceivedAsync(
            hospitalId,
            userId,
            roleName,
            safeTake);

        return ServiceResultDto<List<NotificationStackItemDto>>.Ok(items);
    }

    private static int NormalizeTake(int take)
    {
        if (take <= 0)
        {
            return 20;
        }

        return take > 100 ? 100 : take;
    }
}