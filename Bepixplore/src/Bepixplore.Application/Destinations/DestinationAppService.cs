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

        public override async Task<DestinationDto> CreateAsync(CreateUpdateDestinationDto input)
        {
            // Buscamos ignorando mayúsculas/minúsculas para evitar duplicados "fantasma"
            var existingDestination = await Repository.FirstOrDefaultAsync(x =>
                x.Name.ToLower() == input.Name.ToLower() &&
                x.Country.ToLower() == input.Country.ToLower());

            if (existingDestination != null)
            {
                // Si ya existe, devolvemos la que ya tenemos con su ID real
                return ObjectMapper.Map<Destination, DestinationDto>(existingDestination);
            }

            // Si no existe, la creamos de cero
            var destination = ObjectMapper.Map<CreateUpdateDestinationDto, Destination>(input);
            var insertedDestination = await Repository.InsertAsync(destination, autoSave: true);

            return ObjectMapper.Map<Destination, DestinationDto>(insertedDestination);
        }
    }
}

