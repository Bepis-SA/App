using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using System.Linq;
using Volo.Abp.Data;

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

        public RatingAppService(
        IRepository<Rating, Guid> repository,
        ICurrentUser currentUser,
        IDataFilter dataFilter) : base(repository)
        {
            _currentUser = currentUser;
            _dataFilter = dataFilter;
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
            using (_dataFilter.Disable<IUserOwned>())
            {
                var queryable = await Repository.GetQueryableAsync();

                var query = queryable.Where(x => x.DestinationId == destinationId);

                var ratings = await AsyncExecuter.ToListAsync(query);
                return ObjectMapper.Map<List<Rating>, List<RatingDto>>(ratings);
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
