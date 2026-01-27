using System;
using Volo.Abp.Application.Dtos;

namespace Bepixplore.Experiences
{
    public class TravelExperienceDto : EntityDto<Guid>
    {
        public Guid DestinationId { get; set; }
        public TravelRating Rating { get; set; }
        public string Description { get; set; }
        public DateTime TravelDate { get; set; }
    }
}