using Bepixplore.Application.Contracts.Destinations;
using Bepixplore.Cities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;


namespace Bepixplore.Destinations
{
    public interface IDestinationAppService :
        ICrudAppService<
        DestinationDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateDestinationDto>
    {
        Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request);

    }
}