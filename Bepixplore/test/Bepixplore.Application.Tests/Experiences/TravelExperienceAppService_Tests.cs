using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace Bepixplore.Experiences;

public abstract class TravelExperienceAppService_Tests<TStartupModule> : BepixploreApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected readonly ITravelExperienceAppService _experienceAppService;
    protected readonly IRepository<TravelExperience, Guid> _experienceRepository;
    protected readonly ICurrentUser _currentUser;

    protected TravelExperienceAppService_Tests()
    {
        _experienceAppService = GetRequiredService<ITravelExperienceAppService>();
        _experienceRepository = GetRequiredService<IRepository<TravelExperience, Guid>>();
        _currentUser = GetRequiredService<ICurrentUser>();
    }

    [Fact]
    public async Task GetListAsync_Should_Filter_By_Destination_Keyword_And_Rating()
    {
        // Arrange
        var destId = Guid.NewGuid();
        var myId = _currentUser.GetId();
        var now = DateTime.Now;

        await WithUnitOfWorkAsync(async () =>
        {
            await _experienceRepository.InsertAsync(new TravelExperience(Guid.NewGuid(), destId, myId, TravelRating.Positive, "Increíble viaje a Salta", now));
            await _experienceRepository.InsertAsync(new TravelExperience(Guid.NewGuid(), destId, myId, TravelRating.Negative, "Mala experiencia en el hotel", now));
        });

        // Act
        var result = await _experienceAppService.GetListAsync(new GetTravelExperienceListDto
        {
            DestinationId = destId,
            Rating = (int)TravelRating.Positive
        });

        // Assert
        result.TotalCount.ShouldBe(1);
        result.Items[0].Rating.ShouldBe(TravelRating.Positive);
        result.Items[0].Description.ShouldContain("Increíble");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Description_When_Owner()
    {
        // Arrange
        var id = Guid.NewGuid();
        var destId = Guid.NewGuid();
        var myId = _currentUser.GetId();

        await WithUnitOfWorkAsync(async () =>
        {
            await _experienceRepository.InsertAsync(new TravelExperience(id, destId, myId, TravelRating.Neutral, "Original", DateTime.Now));
        });

        // Act
        var input = new CreateUpdateTravelExperienceDto
        {
            Description = "Editado",
            Rating = TravelRating.Positive,
            DestinationId = destId
        };

        var result = await _experienceAppService.UpdateAsync(id, input);

        // Assert
        result.Description.ShouldBe("Editado");
        result.Rating.ShouldBe(TravelRating.Positive);
    }

    [Fact]
    public async Task DeleteAsync_Should_Fail_If_Not_My_Experience()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var experienceId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _experienceRepository.InsertAsync(new TravelExperience(
                experienceId,
                Guid.NewGuid(),
                otherUserId,
                TravelRating.Neutral,
                "No tocar",
                DateTime.Now));
        });

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _experienceAppService.DeleteAsync(experienceId);
        });
    }

    [Fact]
    public async Task DeleteAsync_Should_Fail_For_Non_Owner()
    {
        // Arrange:
        var otherUserId = Guid.NewGuid();
        var experienceId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _experienceRepository.InsertAsync(new TravelExperience(
                experienceId,
                Guid.NewGuid(),
                otherUserId,
                TravelRating.Negative,
                "No tocar",
                DateTime.Now
            ));
        });

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _experienceAppService.DeleteAsync(experienceId);
        });
    }
}