using Bepixplore.Destinations;
using Bepixplore.Ratings;
using NSubstitute;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
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

        protected RatingAppService_Tests()
        {
            _ratingAppService = GetRequiredService<IRatingAppService>();
            _ratingRepository = GetRequiredService<IRepository<Rating, Guid>>();
            _currentUser = GetRequiredService<ICurrentUser>();
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
            // Arrange
            var currentUserMock = Substitute.For<ICurrentUser>();
            currentUserMock.IsAuthenticated.Returns(false);
            var repositoryMock = Substitute.For<IRepository<Rating, Guid>>();
            var service = new RatingAppService(repositoryMock, currentUserMock);

            // Act & Assert
            await Assert.ThrowsAsync<AbpAuthorizationException>(async () =>
            {
                await service.CreateAsync(new CreateUpdateRatingDto
                {
                    DestinationId = Guid.NewGuid(),
                    Score = 5
                });
            });
        }
    }
}