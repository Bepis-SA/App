using Bepixplore.Notifications;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Users;
using Xunit;

namespace Bepixplore.Notifications;

public abstract class NotificationAppService_Tests<TStartupModule> : BepixploreApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected readonly INotificationAppService _notificationAppService;
    protected readonly IRepository<Notification, Guid> _notificationRepository;
    protected readonly ICurrentUser _currentUser;

    protected NotificationAppService_Tests()
    {
        _notificationAppService = GetRequiredService<INotificationAppService>();
        _notificationRepository = GetRequiredService<IRepository<Notification, Guid>>();
        _currentUser = GetRequiredService<ICurrentUser>();
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Only_Own_Notifications()
    {
        // Arrange
        var myId = _currentUser.GetId();
        var otherUserId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _notificationRepository.InsertAsync(new Notification(Guid.NewGuid(), myId, "Título 1", "Mío", NotificationType.DestinationUpdate));
            await _notificationRepository.InsertAsync(new Notification(Guid.NewGuid(), otherUserId, "Título 2", "Ajeno", NotificationType.DestinationUpdate));
        });

        // Act
        var result = await _notificationAppService.GetListAsync();

        // Assert
        result.Count.ShouldBe(1);
        result[0].Message.ShouldBe("Mío");
    }

    [Fact]
    public async Task MarkAsReadAsync_Should_Update_Status()
    {
        // Arrange
        var notificationId = Guid.NewGuid();
        await WithUnitOfWorkAsync(async () =>
        {
            await _notificationRepository.InsertAsync(new Notification(notificationId, _currentUser.GetId(), "Test", "Msg", NotificationType.DestinationUpdate));
        });

        // Act
        await _notificationAppService.MarkAsReadAsync(notificationId);

        // Assert
        var updated = await _notificationRepository.GetAsync(notificationId);
        updated.IsRead.ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_Exception_If_Not_Owner()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _notificationRepository.InsertAsync(new Notification(notificationId, otherUserId, "Intruso", "Msg", NotificationType.DestinationUpdate));
        });

        // Act & Assert
        await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        {
            await _notificationAppService.DeleteAsync(notificationId);
        });
    }
}