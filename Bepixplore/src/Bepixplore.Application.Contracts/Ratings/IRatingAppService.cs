using System;
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
        Task<PagedResultDto<RatingDto>> GetListByDestinationAsync(Guid destinationId);
        Task<double> GetAverageRatingAsync(Guid destinationId);
    }
}
