using Bepixplore.Application.Contracts.Destinations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Bepixplore.Favorites
{
    public interface IFavoriteAppService : IApplicationService
    {
        Task<DestinationDto> AddAsync(CreateUpdateDestinationDto input);
        Task DeleteAsync(Guid destinationId);
        Task<List<DestinationDto>> GetListAsync();
    }
}