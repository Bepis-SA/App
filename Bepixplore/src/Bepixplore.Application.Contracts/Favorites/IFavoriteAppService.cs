using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
// Importante: Usamos el namespace que te funcionó antes
using Bepixplore.Application.Contracts.Destinations;

namespace Bepixplore.Favorites
{
    public interface IFavoriteAppService : IApplicationService
    {
        Task<DestinationDto> AddAsync(CreateUpdateDestinationDto input);

        // 6.2: Eliminar de favoritos
        Task RemoveAsync(Guid destinationId);

        // 6.3: Consultar lista personal
        Task<List<DestinationDto>> GetListAsync();
    }
}