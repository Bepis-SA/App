using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bepixplore.Favorites;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Bepixplore.Notifications;

public class NotificationManager : DomainService
{
    private readonly IRepository<Notification, Guid> _notificationRepository;
    private readonly IRepository<Favorite, Guid> _favoriteRepository;

    public NotificationManager(
        IRepository<Notification, Guid> notificationRepository,
        IRepository<Favorite, Guid> favoriteRepository)
    {
        _notificationRepository = notificationRepository;
        _favoriteRepository = favoriteRepository;
    }

    // Este método centraliza la lógica que ya tenías
    public async Task CreateFavoriteNotificationsAsync(
        Guid destinationId,
        string title,
        string message,
        NotificationType type,
        Guid? excludeUserId = null)
    {
        var favorites = await _favoriteRepository.GetListAsync(f => f.DestinationId == destinationId);

        foreach (var fav in favorites)
        {
            if (excludeUserId.HasValue && fav.UserId == excludeUserId.Value) continue;

            await _notificationRepository.InsertAsync(new Notification(
                GuidGenerator.Create(),
                fav.UserId,
                title,
                message,
                type
            ));
        }
    }
}