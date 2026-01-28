using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
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

        public RatingAppService(IRepository<Rating, Guid> repository, ICurrentUser currentUser)
            : base(repository)
        {
            _currentUser = currentUser;
        }

        public override async Task<RatingDto> CreateAsync(CreateUpdateRatingDto input)
        {
            if (!_currentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException();
            }

            var entity = ObjectMapper.Map<CreateUpdateRatingDto, Rating>(input);
            entity.UserId = _currentUser.GetId();

            var existingRating = await Repository.FirstOrDefaultAsync(r =>
                r.DestinationId == entity.DestinationId &&
                r.UserId == entity.UserId);

            if (existingRating != null)
            {
                throw new UserFriendlyException("You have already rated this destination.");
            }

            await Repository.InsertAsync(entity);
            return ObjectMapper.Map<Rating, RatingDto>(entity);
        }

        public async Task<List<RatingDto>> GetListByDestinationAsync(Guid destinationId)
        {
            var ratings = await Repository.GetListAsync(r => r.DestinationId == destinationId);

            return ObjectMapper.Map<List<Rating>, List<RatingDto>>(ratings);
        }
    }

}
