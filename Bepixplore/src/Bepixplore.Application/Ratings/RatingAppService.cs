using Bepixplore.Destinations;
using Bepixplore.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Bepixplore.Ratings
{
    [Authorize]
    public class RatingAppService :
        CrudAppService<
            Rating,
            RatingDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateRatingDto>,
            IRatingAppService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IDataFilter _dataFilter;
        private readonly INotificationAppService _notificationAppService;
        private readonly IRepository<Destination, Guid> _destinationRepository;

        public RatingAppService(
        IRepository<Rating, Guid> repository,
        ICurrentUser currentUser,
        IDataFilter dataFilter,
        INotificationAppService notificationAppService,
        IRepository<Destination, Guid> destinationRepository) : base(repository)
        {
            _currentUser = currentUser;
            _dataFilter = dataFilter;
            _notificationAppService = notificationAppService;
            _destinationRepository = destinationRepository;
        }

        [Authorize]
        public override async Task<RatingDto> CreateAsync(CreateUpdateRatingDto input)
        {
            if (!_currentUser.IsAuthenticated) throw new AbpAuthorizationException();

            var currentUserId = _currentUser.GetId();

            var existingRating = await Repository.FirstOrDefaultAsync(r =>
                r.DestinationId == input.DestinationId &&
                r.UserId == currentUserId);

            if (existingRating != null)
            {
                throw new UserFriendlyException("Ya has calificado este destino...");
            }

            var newRating = ObjectMapper.Map<CreateUpdateRatingDto, Rating>(input);
            newRating.UserId = currentUserId;

            await Repository.InsertAsync(newRating, autoSave: true);

            try
            {
                var destination = await _destinationRepository.GetAsync(input.DestinationId);

                await _notificationAppService.NotifyNewRatingAsync(
                    destination.Id,
                    destination.Name,
                    newRating.Score,
                    _currentUser.Name ?? "Un viajero"
                );
            }
            catch (Exception ex)
            {
                Logger.LogWarning("No se pudo enviar la notificación de rating: " + ex.Message);
            }

            return ObjectMapper.Map<Rating, RatingDto>(newRating);
        }

        public async Task<PagedResultDto<RatingDto>> GetListByDestinationAsync(Guid destinationId)
        {
            using (_dataFilter.Disable<IUserOwned>())
            {
                var queryable = await Repository.GetQueryableAsync();
                var query = queryable.Where(x => x.DestinationId == destinationId);

                var totalCount = await AsyncExecuter.CountAsync(query); 
                var ratings = await AsyncExecuter.ToListAsync(query); 
                var dtos = ObjectMapper.Map<List<Rating>, List<RatingDto>>(ratings);
                return new PagedResultDto<RatingDto>(totalCount, dtos);
            }
        }

        public async Task<double> GetAverageRatingAsync(Guid destinationId)
        {
            using (_dataFilter.Disable<IUserOwned>())
            {
                var queryable = await Repository.GetQueryableAsync();
                var ratings = queryable.Where(x => x.DestinationId == destinationId);

                if (!await AsyncExecuter.AnyAsync(ratings))
                {
                    return 0;
                }

                return await AsyncExecuter.AverageAsync(ratings, x => x.Score);
            }
        }
    }

}
