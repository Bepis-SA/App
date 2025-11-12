using System;
using Volo.Abp.Application.Dtos;

namespace Bepixplore.Ratings
{
    public class RatingDto : AuditedEntityDto<Guid>
    {
        public Guid UserId { get; set; }
        public Guid DestinationId { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }
    }
}
