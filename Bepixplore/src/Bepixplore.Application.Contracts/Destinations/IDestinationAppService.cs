using Bepixplore.Application.Contracts.Destinations;
using System;
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

    }
}