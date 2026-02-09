using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Bepixplore.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task<PublicUserProfileDto> GetPublicProfileAsync(string userName);

        Task DeleteMyAccountAsync();
    }
}
