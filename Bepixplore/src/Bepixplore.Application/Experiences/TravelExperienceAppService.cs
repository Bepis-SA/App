using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Bepixplore.Experiences
{
    public class TravelExperienceAppService : ApplicationService, ITravelExperienceAppService
    {
        private readonly IRepository<TravelExperience, Guid> _repository;

        public TravelExperienceAppService(IRepository<TravelExperience, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<TravelExperienceDto> CreateAsync(CreateUpdateTravelExperienceDto input)
        {
            var experience = new TravelExperience(
                GuidGenerator.Create(),
                input.DestinationId,
                CurrentUser.GetId(),
                input.Rating,
                input.Description,
                input.TravelDate
            );

            await _repository.InsertAsync(experience);
            return ObjectMapper.Map<TravelExperience, TravelExperienceDto>(experience);
        }

        // 4.6: Implementación del buscador por descripción
        public async Task<List<TravelExperienceDto>> GetListAsync(string keyword)
        {
            var queryable = await _repository.GetQueryableAsync();

            var experiences = queryable
                .WhereIf(!string.IsNullOrWhiteSpace(keyword), x => x.Description.Contains(keyword))
                .ToList();

            return ObjectMapper.Map<List<TravelExperience>, List<TravelExperienceDto>>(experiences);
        }

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);
    }
}