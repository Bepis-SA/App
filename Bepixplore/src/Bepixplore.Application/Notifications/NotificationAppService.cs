using Bepixplore;
using Bepixplore.Notifications;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

[Authorize]
public class NotificationAppService : BepixploreAppService, INotificationAppService
{
    private readonly IRepository<Notification, Guid> _notificationRepository;
    private readonly INotificationAppService _notificationAppService;

    public NotificationAppService(IRepository<Notification, Guid> notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationDto>> GetListAsync()
    {
        var userId = CurrentUser.GetId();

        var notifications = await _notificationRepository.GetListAsync(n => n.UserId == userId);

        return ObjectMapper.Map<List<Notification>, List<NotificationDto>>(notifications);
    }

    public async Task DeleteAsync(Guid id)
    {
        var notification = await _notificationRepository.GetAsync(id);

        if (notification.UserId != CurrentUser.GetId())
        {
            throw new UserFriendlyException("No tienes permiso para eliminar esta notificación.");
        }

        await _notificationRepository.DeleteAsync(id);
    }

    public async Task CreateNotificationAsync(Guid targetUserId, string title, string message, NotificationType type)
    {
        var notification = new Notification(
            id: GuidGenerator.Create(),
            userId: targetUserId,
            title: title,
            message: message,
            type: type
        );

        await _notificationRepository.InsertAsync(notification);
    }

    public async Task NotifyDestinationUpdateAsync(Guid destinationId, string destinationName)
    {
        var currentUserId = CurrentUser.GetId();

        var notification = new Notification(
            GuidGenerator.Create(),
            currentUserId,
            "📍 Destino Actualizado",
            $"La información de '{destinationName}' ha sido actualizada.",
            NotificationType.DestinationUpdate
        );

        await _notificationRepository.InsertAsync(notification);
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var notification = await _notificationRepository.GetAsync(id);

        if (notification.UserId != CurrentUser.GetId())
        {
            throw new UserFriendlyException("No tienes permiso para marcar esta notificación.");
        }

        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification);
    }
}
