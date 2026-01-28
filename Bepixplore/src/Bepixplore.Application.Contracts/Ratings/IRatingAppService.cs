using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Bepixplore.Ratings
{
    public interface IRatingAppService :
    ICrudAppService<
        RatingDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateRatingDto>
    {
        Task<List<RatingDto>> GetListByDestinationAsync(Guid destinationId);
    }
}
