using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Bepixplore.Experiences
{
    public interface ITravelExperienceAppService : IApplicationService
    {
        Task<TravelExperienceDto> CreateAsync(CreateUpdateTravelExperienceDto input);

        Task<PagedResultDto<TravelExperienceDto>> GetListAsync(GetTravelExperienceListDto input);

        Task<TravelExperienceDto> UpdateAsync(Guid id, CreateUpdateTravelExperienceDto input);
        Task DeleteAsync(Guid id);
    }
}