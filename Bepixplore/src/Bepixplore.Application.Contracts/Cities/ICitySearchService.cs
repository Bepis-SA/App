using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Bepixplore.Cities
{
    public interface ICitySearchService : IApplicationService
    {
        Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request);
    }
}