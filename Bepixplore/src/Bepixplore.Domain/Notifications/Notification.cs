using System;
using Volo.Abp.Domain.Entities;

namespace Bepixplore.Notifications
{
    public class Notification : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreationTime { get; set; }

        protected Notification() { }

        public Notification(Guid id, Guid userId, string title, string message, NotificationType type)
            : base(id)
        {
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            IsRead = false;
            CreationTime = DateTime.UtcNow;
        }
    }
}