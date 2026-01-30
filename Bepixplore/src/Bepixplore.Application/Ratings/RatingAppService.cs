using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
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

        public RatingAppService(IRepository<Rating, Guid> repository, ICurrentUser currentUser)
            : base(repository)
        {
            _currentUser = currentUser;
        }
        public override async Task<RatingDto> CreateAsync(CreateUpdateRatingDto input)
        {
            var existingRating = await Repository.FirstOrDefaultAsync(r =>
                r.DestinationId == input.DestinationId &&
                r.UserId == _currentUser.Id);

            if (existingRating != null)
            {
                existingRating.Score = input.Score;
                existingRating.Comment = input.Comment;
                await Repository.UpdateAsync(existingRating, autoSave: true);
                return ObjectMapper.Map<Rating, RatingDto>(existingRating);
            }

            var newRating = ObjectMapper.Map<CreateUpdateRatingDto, Rating>(input);
            newRating.UserId = _currentUser.Id.Value;

            await Repository.InsertAsync(newRating, autoSave: true);
            return ObjectMapper.Map<Rating, RatingDto>(newRating);
        }
        public async Task<List<RatingDto>> GetListByDestinationAsync(Guid destinationId)
        {
            var ratings = await Repository.GetListAsync(r => r.DestinationId == destinationId);
            return ObjectMapper.Map<List<Rating>, List<RatingDto>>(ratings);
        }
    }
}
/*      public RatingAppService(IRepository<Rating, Guid> repository, ICurrentUser currentUser)
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
*/