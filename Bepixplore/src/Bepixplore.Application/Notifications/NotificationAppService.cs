using Bepixplore.Destinations;
using Bepixplore.Favorites;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Bepixplore.Notifications
{
    public class NotificationAppService : BepixploreAppService, INotificationAppService
    {
        private readonly IRepository<Notification, Guid> _notificationRepository;
        private readonly IRepository<Favorite, Guid> _favoriteRepository;
        private readonly IRepository<Destination, Guid> _destinationRepository;

        public NotificationAppService(
            IRepository<Notification, Guid> notificationRepository,
            IRepository<Favorite, Guid> favoriteRepository,
            IRepository<Destination, Guid> destinationRepository)
        {
            _notificationRepository = notificationRepository;
            _favoriteRepository = favoriteRepository;
            _destinationRepository = destinationRepository;
        }

        [Authorize]
        public async Task<List<NotificationDto>> GetListAsync()
        {
            var userId = CurrentUser.GetId();
            var notifications = await _notificationRepository.GetListAsync(n => n.UserId == userId);
            var sortedNotifications = notifications.OrderByDescending(n => n.CreationTime).ToList();
            return ObjectMapper.Map<List<Notification>, List<NotificationDto>>(sortedNotifications);
        }

        [Authorize]
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

        [Authorize]
        public async Task DeleteAsync(Guid id)
        {
            var notification = await _notificationRepository.GetAsync(id);
            if (notification.UserId != CurrentUser.GetId())
            {
                throw new UserFriendlyException("No tienes permiso para eliminar esta notificación.");
            }
            await _notificationRepository.DeleteAsync(id);
        }

        public async Task NotifyNewRatingAsync(Guid destinationId, string destinationName, int stars, string creatorName)
        {
            var favorites = await _favoriteRepository.GetListAsync(f => f.DestinationId == destinationId);
            var currentUserId = CurrentUser.Id;

            foreach (var fav in favorites)
            {
                if (currentUserId.HasValue && fav.UserId == currentUserId.Value) continue;

                await _notificationRepository.InsertAsync(new Notification(
                    GuidGenerator.Create(),
                    fav.UserId,
                    "⭐ Nueva Comentario",
                    $"Nuevo comentario en '{destinationName}'.",
                    NotificationType.Comment
                ));
            }
        }
        public async Task NotifyNewEventAsync(string cityName, string eventName, string venueName, string startDate)
        {
            var destinationIds = (await _destinationRepository.GetListAsync(d => d.City == cityName))
                                 .Select(d => d.Id)
                                 .ToList();

            if (!destinationIds.Any()) return;

            var favorites = await _favoriteRepository.GetListAsync(f =>
                destinationIds.Contains(f.DestinationId));

            foreach (var fav in favorites)
            {
                string mensaje = $"¡Nuevo evento en {cityName}! '{eventName}' en {venueName} el día {startDate}.";

                var alreadyNotified = await _notificationRepository.AnyAsync(n =>
                    n.UserId == fav.UserId &&
                    n.Message == mensaje);

                if (alreadyNotified) continue;

                await _notificationRepository.InsertAsync(new Notification(
                    GuidGenerator.Create(),
                    fav.UserId,
                    "🎫 ¡Nuevo Evento!",
                    mensaje,
                    NotificationType.NewEvent
                ));
            }
        }
        public async Task NotifyDestinationUpdateAsync(Guid destinationId, string destinationName)
        {
            var favorites = await _favoriteRepository.GetListAsync(f => f.DestinationId == destinationId);

            foreach (var fav in favorites)
            {
                await _notificationRepository.InsertAsync(new Notification(
                    GuidGenerator.Create(),
                    fav.UserId,
                    "📍 Destino Actualizado",
                    $"El destino '{destinationName}' ha sido actualizado.",
                    NotificationType.DestinationUpdate
                ));
            }
        }
    }
}