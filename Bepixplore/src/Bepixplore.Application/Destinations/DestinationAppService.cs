using Bepixplore.Application.Contracts.Destinations;
using Bepixplore.Cities;
using Bepixplore.Notifications;
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
        private readonly INotificationAppService _notificationAppService;

        public DestinationAppService(
            IRepository<Destination, Guid> repository,
            ICitySearchService citySearchService,
            INotificationAppService notificationAppService)
            : base(repository)
        {
            _citySearchService = citySearchService;
            _notificationAppService = notificationAppService;
        }
        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            return await _citySearchService.SearchCitiesAsync(request);
        }

        public override async Task<DestinationDto> CreateAsync(CreateUpdateDestinationDto input)
        {
            var existingDestination = await Repository.FirstOrDefaultAsync(x =>
                x.Name.ToLower() == input.Name.ToLower() &&
                x.Country.ToLower() == input.Country.ToLower());

            if (existingDestination != null)
            {
                return ObjectMapper.Map<Destination, DestinationDto>(existingDestination);
            }

            var destination = ObjectMapper.Map<CreateUpdateDestinationDto, Destination>(input);
            var insertedDestination = await Repository.InsertAsync(destination, autoSave: true);

            return ObjectMapper.Map<Destination, DestinationDto>(insertedDestination);
        }

        public override async Task<DestinationDto> UpdateAsync(Guid id, CreateUpdateDestinationDto input)
        {
            var destination = await base.UpdateAsync(id, input);

            await _notificationAppService.NotifyDestinationUpdateAsync(id, destination.Name);

            return destination;
        }
    }
}
