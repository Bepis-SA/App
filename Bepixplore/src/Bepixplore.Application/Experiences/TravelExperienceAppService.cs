using Bepixplore.Ratings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Bepixplore.Experiences
{
    public class TravelExperienceAppService :
    CrudAppService<
        TravelExperience,
        TravelExperienceDto,
        Guid,
        GetTravelExperienceListDto,
        CreateUpdateTravelExperienceDto>,
        ITravelExperienceAppService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IDataFilter _dataFilter;

        public TravelExperienceAppService(
            IRepository<TravelExperience, Guid> repository,
            ICurrentUser currentUser,
            IDataFilter dataFilter) : base(repository)
        {
            _currentUser = currentUser;
            _dataFilter = dataFilter;
        }

        public override async Task<PagedResultDto<TravelExperienceDto>> GetListAsync(GetTravelExperienceListDto input)
        {
            using (_dataFilter.Disable<IUserOwned>())
            {
                var queryable = await Repository.GetQueryableAsync();

                var query = queryable
                    .Where(x => x.DestinationId == input.DestinationId)
                    .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Description.Contains(input.Keyword))
                    .WhereIf(input.Rating.HasValue, x => x.Rating == (TravelRating)input.Rating.Value);

                // Obtenemos el total para la paginación
                var totalCount = await AsyncExecuter.CountAsync(query);

                // Obtenemos la lista de datos
                var experiences = await AsyncExecuter.ToListAsync(query);

                // 2. Mapeamos la lista
                var dtos = ObjectMapper.Map<List<TravelExperience>, List<TravelExperienceDto>>(experiences);

                // 3. Devolvemos el objeto que Swagger espera para no tirar error 500
                return new PagedResultDto<TravelExperienceDto>(totalCount, dtos);
            }
        }
        public override async Task DeleteAsync(Guid id)
        {
            var experience = await Repository.GetAsync(id);

            if (experience.UserId != _currentUser.GetId())
            {
                throw new UserFriendlyException("No tienes permiso para eliminar esta experiencia.");
            }

            await Repository.DeleteAsync(id);
        }

        public override async Task<TravelExperienceDto> UpdateAsync(Guid id, CreateUpdateTravelExperienceDto input)
        {
            var experience = await Repository.GetAsync(id);

            if (experience.UserId != _currentUser.GetId())
            {
                throw new UserFriendlyException("No tienes permiso para editar esta experiencia.");
            }

            ObjectMapper.Map(input, experience);

            await Repository.UpdateAsync(experience);
            return ObjectMapper.Map<TravelExperience, TravelExperienceDto>(experience);
        }
    }
}