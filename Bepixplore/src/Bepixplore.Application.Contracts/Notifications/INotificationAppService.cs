using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using System.Collections.Generic;

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