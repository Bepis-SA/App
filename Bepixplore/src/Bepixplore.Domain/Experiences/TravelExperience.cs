using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Bepixplore.Experiences
{
    public class TravelExperience : FullAuditedAggregateRoot<Guid>
    {
        public Guid DestinationId { get; set; } // 4.1: Vínculo con destino
        public Guid UserId { get; set; }
        public TravelRating Rating { get; set; } // 4.5: Valoración
        public string Description { get; set; } // Para búsqueda por palabras clave
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