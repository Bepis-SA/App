using Bepixplore.Application.Contracts.Destinations;
using Bepixplore.Destinations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace Bepixplore.Favorites;

public abstract class FavoriteAppService_Tests<TStartupModule> : BepixploreApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected readonly IFavoriteAppService _favoriteAppService;
    protected readonly IRepository<Favorite, Guid> _favoriteRepository;
    protected readonly IRepository<Destination, Guid> _destinationRepository;
    protected readonly ICurrentUser _currentUser;

    protected FavoriteAppService_Tests()
    {
        _favoriteAppService = GetRequiredService<IFavoriteAppService>();
        _favoriteRepository = GetRequiredService<IRepository<Favorite, Guid>>();
        _destinationRepository = GetRequiredService<IRepository<Destination, Guid>>();
        _currentUser = GetRequiredService<ICurrentUser>();
    }

    [Fact]
    public async Task AddAsync_Should_Create_Destination_And_Favorite_If_New()
    {
        // Arrange
        var input = new CreateUpdateDestinationDto
        {
            Name = "Cataratas",
            Country = "Argentina",
            City = "Iguazú",
            Population = 100000,
            Coordinates = new CoordinatesDto { Latitude = -25.6f, Longitude = -54.4f }
        };

        // Act
        await _favoriteAppService.AddAsync(input);

        // Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var dest = await _destinationRepository.FirstOrDefaultAsync(x => x.Name == "Cataratas");
            dest.ShouldNotBeNull();

            var fav = await _favoriteRepository.FirstOrDefaultAsync(x =>
                x.UserId == _currentUser.GetId() && x.DestinationId == dest.Id);
            fav.ShouldNotBeNull();
        });
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Only_My_Favorites()
    {
        // Arrange
        var myId = _currentUser.GetId();
        var otherUserId = Guid.NewGuid();
        var destId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _destinationRepository.InsertAsync(new Destination(destId, "Destino Test", "Arg", "City", 10, "f.jpg", DateTime.Now, new Coordinates(0, 0)));

            await _favoriteRepository.InsertAsync(new Favorite(Guid.NewGuid(), myId, destId));
            await _favoriteRepository.InsertAsync(new Favorite(Guid.NewGuid(), otherUserId, destId));
        });

        // Act
        var result = await _favoriteAppService.GetListAsync();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("Destino Test");
    }

    [Fact]
    public async Task RemoveAsync_Should_Delete_Favorite_Link()
    {
        // Arrange
        var myId = _currentUser.GetId();
        var destId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _destinationRepository.InsertAsync(new Destination(destId, "Borrar", "Arg", "City", 10, "f.jpg", DateTime.Now, new Coordinates(0, 0)));
            await _favoriteRepository.InsertAsync(new Favorite(Guid.NewGuid(), myId, destId));
        });

        // Act
        await _favoriteAppService.RemoveAsync(destId);

        // Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var fav = await _favoriteRepository.FirstOrDefaultAsync(x =>
                x.UserId == myId && x.DestinationId == destId);
            fav.ShouldBeNull();
        });
    }
}