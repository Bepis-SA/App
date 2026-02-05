using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Bepixplore.Notifications
{
    public interface INotificationAppService : IApplicationService
    {
        Task NotifyDestinationUpdateAsync(Guid destinationId, string destinationName);
        Task<List<NotificationDto>> GetListAsync();
        Task MarkAsReadAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}