using System;
using Volo.Abp.Application.Dtos;

namespace Bepixplore.Experiences
{
    public class GetTravelExperienceListDto : PagedAndSortedResultRequestDto
    {
        public Guid DestinationId { get; set; }
        public string? Keyword { get; set; }
        public int? Rating { get; set; }
    }
}