using System.Threading.Tasks;

namespace Bepixplore.Cities
{
    public interface ICitySearchService
    {
        Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request);
    }
}