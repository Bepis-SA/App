using Volo.Abp.Application.Dtos;
using System;

namespace Bepixplore.Experiences
{
    public class GetTravelExperienceListDto : PagedAndSortedResultRequestDto
    {
        public Guid DestinationId { get; set; }
        public string? Keyword { get; set; }
        public int? Rating { get; set; }
    }
}