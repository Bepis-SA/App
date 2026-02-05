using System;

namespace Bepixplore.Experiences
{
    public class CreateUpdateTravelExperienceDto
    {
        public Guid DestinationId { get; set; }
        public TravelRating Rating { get; set; }
        public string Description { get; set; }
        public DateTime TravelDate { get; set; }
    }
}