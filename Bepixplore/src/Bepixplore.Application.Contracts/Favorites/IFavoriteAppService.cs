using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Bepixplore.Application.Contracts.Destinations;

namespace Bepixplore.Favorites
{
    public interface IFavoriteAppService : IApplicationService
    {
        Task<DestinationDto> AddAsync(CreateUpdateDestinationDto input);
        Task RemoveAsync(Guid destinationId);
        Task<List<DestinationDto>> GetListAsync();
    }
}