using Bepixplore.Destinations;
using Bepixplore.Ratings;
using NSubstitute;
using Shouldly;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace Bepixplore.Application.Tests.Ratings
{
    public abstract class RatingAppService_Tests<TStartupModule> : BepixploreApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        protected readonly IRatingAppService _ratingAppService;
        protected readonly IRepository<Rating, Guid> _ratingRepository;
        protected readonly ICurrentUser _currentUser;
        private readonly IDataFilter _dataFilter;

        protected RatingAppService_Tests()
        {
            _ratingAppService = GetRequiredService<IRatingAppService>();
            _ratingRepository = GetRequiredService<IRepository<Rating, Guid>>();
            _currentUser = GetRequiredService<ICurrentUser>();
            _dataFilter = GetRequiredService<IDataFilter>();
        }

        [Fact]
        public async Task Should_Only_Return_Current_User_Ratings()
        {
            // Arrange
            var user1Id = _currentUser.GetId();
            var user2Id = Guid.NewGuid();
            var destinationId = Guid.NewGuid();

            await WithUnitOfWorkAsync(async () =>
            {
                var destinationRepo = GetRequiredService<IRepository<Destination, Guid>>();
                await destinationRepo.InsertAsync(new Destination(
                    id: destinationId,
                    name: "Buenos Aires",
                    country: "Argentina",
                    city: "Buenos Aires",
                    population: 15000000,
                    photo: "photo.jpg",
                    updateDate: DateTime.UtcNow,
                    coordinates: new Coordinates(latitude: -34.6037f, longitude: -58.3816f)
                ));
            });

            await WithUnitOfWorkAsync(async () =>
            {
                await _ratingRepository.InsertAsync(new Rating(
                    id: Guid.NewGuid(),
                    userId: user1Id,
                    destinationId: destinationId,
                    score: 5
                ));

                await _ratingRepository.InsertAsync(new Rating(
                    id: Guid.NewGuid(),
                    userId: user2Id,
                    destinationId: destinationId,
                    score: 3
                ));
            });

            // Act
            var result = await _ratingAppService.GetListAsync(new PagedAndSortedResultRequestDto());

            // Assert
            result.Items.Count.ShouldBe(1);
            result.Items[0].UserId.ShouldBe(user1Id);
        }

        [Fact]
        public async Task CreateAsync_Should_Require_Authentication()
        {
            var principalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();

            var anonymousPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            using (principalAccessor.Change(anonymousPrincipal))
            {
                // Arrange
                var input = new CreateUpdateRatingDto
                {
                    DestinationId = Guid.NewGuid(),
                    Score = 5,
                    Comment = "Excelente"
                };

                // Act & Assert
                await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
                {
                    await _ratingAppService.CreateAsync(input);
                });
            }
        }

        [Fact]
        public async Task GetAverageRating_Should_Return_Mathematically_Correct_Value()
        {
            // Arrange
            var destinationId = Guid.NewGuid();
            var user1Id = _currentUser.GetId();
            var user2Id = Guid.NewGuid();
            var user3Id = Guid.NewGuid();

            await WithUnitOfWorkAsync(async () =>
            {
                var destinationRepo = GetRequiredService<IRepository<Destination, Guid>>();
                await destinationRepo.InsertAsync(new Destination(
                    destinationId, "Cataratas del Iguazú", "Argentina", "Misiones", 100, "iguazu.jpg", DateTime.Now, new Coordinates(0, 0)));
                await _ratingRepository.InsertAsync(new Rating(Guid.NewGuid(), user1Id, destinationId, 5));
                await _ratingRepository.InsertAsync(new Rating(Guid.NewGuid(), user2Id, destinationId, 4));
                await _ratingRepository.InsertAsync(new Rating(Guid.NewGuid(), user3Id, destinationId, 2));
            });

            // Act
            var average = await _ratingAppService.GetAverageRatingAsync(destinationId);

            // Assert
            average.ShouldBe(3.66, 0.01);
        }
    }
}