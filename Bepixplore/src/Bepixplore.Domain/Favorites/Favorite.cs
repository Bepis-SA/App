using System;
using Volo.Abp.Domain.Entities;

namespace Bepixplore.Favorites
{
    public class Favorite : AggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public Guid DestinationId { get; set; }

        protected Favorite() { }

        public Favorite(Guid id, Guid userId, Guid destinationId) : base(id)
        {
            UserId = userId;
            DestinationId = destinationId;
        }
    }
}