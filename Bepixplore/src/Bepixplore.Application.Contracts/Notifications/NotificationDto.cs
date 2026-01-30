using System;
using Volo.Abp.Application.Dtos;

namespace Bepixplore.Notifications
{
    public class NotificationDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreationTime { get; set; }
        public NotificationType Type { get; set; }
    }
}