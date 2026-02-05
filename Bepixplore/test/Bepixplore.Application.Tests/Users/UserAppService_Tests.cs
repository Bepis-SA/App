using Microsoft.AspNetCore.Identity;
using Shouldly;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace Bepixplore.Users
{
    public abstract class UserAppService_Tests<TStartupModule> : BepixploreApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IUserAppService _userAppService;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IdentityUserManager _userManager;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

        public UserAppService_Tests()
        {
            _userAppService = GetRequiredService<IUserAppService>();
            _userRepository = GetRequiredService<IIdentityUserRepository>();
            _currentUser = GetRequiredService<ICurrentUser>();
            _userManager = GetRequiredService<IdentityUserManager>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }

        [Fact]
        public async Task Should_Get_Public_Profile()
        {
            // Arrange
            var targetUserName = "admin";

            // Act
            var publicProfile = await _userAppService.GetPublicProfileAsync(targetUserName);

            // Assert
            publicProfile.ShouldNotBeNull();
            publicProfile.UserName.ShouldBe(targetUserName);
            publicProfile.CreationTime.ShouldNotBe(default);
        }

        [Fact]
        public async Task Should_Delete_Account()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tempUser = new IdentityUser(userId, "userborrar", "userborrar@test.com");
            (await _userManager.CreateAsync(tempUser, "123456Aa!")).CheckErrors();

            // Act
            var claims = new[] { new Claim(AbpClaimTypes.UserId, tempUser.Id.ToString()) };

            using (_currentPrincipalAccessor.Change(new ClaimsPrincipal(new ClaimsIdentity(claims))))
            {
                await _userAppService.DeleteMyAccountAsync();
            }

            // Assert
            var deletedUser = await _userRepository.FindAsync(userId);
            deletedUser.ShouldBeNull();
        }
    }
}