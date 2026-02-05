using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Bepixplore.Experiences
{
    public class TravelExperience : FullAuditedAggregateRoot<Guid>
    {
        public Guid DestinationId { get; set; }
        public Guid UserId { get; set; }
        public TravelRating Rating { get; set; }
        public string Description { get; set; }
        public DateTime TravelDate { get; set; }

        protected TravelExperience() { }

        public TravelExperience(Guid id, Guid destinationId, Guid userId, TravelRating rating, string description, DateTime travelDate)
            : base(id)
        {
            DestinationId = destinationId;
            UserId = userId;
            Rating = rating;
            Description = description;
            TravelDate = travelDate;
        }
    }
}