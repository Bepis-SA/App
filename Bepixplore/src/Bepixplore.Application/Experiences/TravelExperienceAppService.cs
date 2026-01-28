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
        public async Task<List<TravelExperienceDto>> GetListAsync(GetTravelExperienceListDto input)
        {
            var queryable = await _repository.GetQueryableAsync();

            var query = queryable
                .Where(x => x.DestinationId == input.DestinationId)
                // Filtra por palabra clave si existe
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Description.Contains(input.Keyword))
                // Opción recomendada: Convertir el número al tipo del Enum
                .WhereIf(input.Rating.HasValue, x => x.Rating == (TravelRating)input.Rating.Value);

            var experiences = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<TravelExperience>, List<TravelExperienceDto>>(experiences);
        }

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<TravelExperienceDto> UpdateAsync(Guid id, CreateUpdateTravelExperienceDto input)
        {
            var experience = await _repository.GetAsync(id);

            experience.Description = input.Description;
            experience.Rating = input.Rating;

            await _repository.UpdateAsync(experience);
            return ObjectMapper.Map<TravelExperience, TravelExperienceDto>(experience);
        }
    }
}