using Bepixplore.Application.Contracts.Destinations;
using Bepixplore.Cities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Bepixplore.Destinations
{
    public class DestinationAppService :
        CrudAppService<
        Destination,
        DestinationDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateDestinationDto>,
    IDestinationAppService
    {
        private readonly ICitySearchService _citySearchService;
        public DestinationAppService(IRepository<Destination, Guid> repository, ICitySearchService citySearchService)
            : base(repository)
        {
            _citySearchService = citySearchService;
        }

        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            return await _citySearchService.SearchCitiesAsync(request);
        }
    }
}
