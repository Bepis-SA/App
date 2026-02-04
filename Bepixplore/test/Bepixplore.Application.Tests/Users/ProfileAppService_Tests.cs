using Microsoft.AspNetCore.Identity;
using Shouldly;
using System;
using System.Security.Claims; // Necesario para Claims
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims; // Necesario para AbpClaimTypes
using Xunit;

namespace Bepixplore.Users
{
    public abstract class ProfileAppService_Tests<TStartupModule> : BepixploreApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IProfileAppService _profileAppService;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IdentityUserManager _userManager; // Para crear el usuario
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor; // Para suplantarlo

        public ProfileAppService_Tests()
        {
            _profileAppService = GetRequiredService<IProfileAppService>();
            _userRepository = GetRequiredService<IIdentityUserRepository>();
            _userManager = GetRequiredService<IdentityUserManager>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        }

        [Fact]
        public async Task Should_Update_My_Profile_Extra_Properties()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new IdentityUser(userId, "testuser", "testuser@abp.io");
            (await _userManager.CreateAsync(user, "123456Aa!")).CheckErrors();
            var newPic = "https://bepixplore.com/photo.png";
            var newChannel = NotificationChannel.Screen;
            var newFrequency = NotificationFrequency.WeeklySummary;
            var claims = new[] { new Claim(AbpClaimTypes.UserId, userId.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            using (_currentPrincipalAccessor.Change(principal))
            {
                var currentProfileDto = await _profileAppService.GetAsync();

                var input = new UpdateProfileDto
                {
                    UserName = currentProfileDto.UserName,
                    Email = currentProfileDto.Email,
                    Name = "NombreUpdated",
                    Surname = "ApellidoUpdated",
                    PhoneNumber = "999888777",
                };

                input.ExtraProperties["ProfilePictureUrl"] = newPic;
                input.ExtraProperties["NotificationChannel"] = (int)newChannel;
                input.ExtraProperties["NotificationFrequency"] = (int)newFrequency;
                await _profileAppService.UpdateAsync(input);
            }

            // Assert
            var userInDb = await _userRepository.GetAsync(userId);

            userInDb.Name.ShouldBe("NombreUpdated");
            userInDb.GetProperty<string>("ProfilePictureUrl").ShouldBe(newPic);
            userInDb.GetProperty<int>("NotificationChannel").ShouldBe((int)newChannel);
            userInDb.GetProperty<int>("NotificationFrequency").ShouldBe((int)newFrequency);
        }
    }
}