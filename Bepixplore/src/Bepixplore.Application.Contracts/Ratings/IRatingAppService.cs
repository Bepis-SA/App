using System;
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
    }
}
