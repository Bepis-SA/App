using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Bepixplore.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        Task NotifyDestinationUpdateAsync(Guid destinationId, string destinationName);
        Task NotifyNewRatingAsync(Guid destinationId, string destinationName, int stars, string creatorName);
        Task NotifyNewEventAsync(string cityName, string eventName, string venueName, string startDate);
        Task<List<NotificationDto>> GetListAsync();
        Task MarkAsReadAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}