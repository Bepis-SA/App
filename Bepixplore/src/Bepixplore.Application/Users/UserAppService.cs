using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Bepixplore.Users
{
    public class UserAppService : BepixploreAppService, IUserAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IIdentityUserRepository _userRepository;

        public UserAppService(
            IdentityUserManager userManager,
            IIdentityUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<PublicUserProfileDto> GetPublicProfileAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UserFriendlyException("El usuario no existe.");
            }

            return ObjectMapper.Map<IdentityUser, PublicUserProfileDto>(user);
        }

        public async Task DeleteMyAccountAsync()
        {
            var user = await _userManager.GetByIdAsync(CurrentUser.GetId());

            if (await _userManager.IsInRoleAsync(user, "admin"))
            {
                throw new UserFriendlyException("Los administradores no pueden auto-eliminarse por seguridad.");
            }

            (await _userManager.DeleteAsync(user)).CheckErrors();
        }
    }
}
