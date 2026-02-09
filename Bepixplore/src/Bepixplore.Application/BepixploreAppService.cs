using Bepixplore.Localization;
using Volo.Abp.Application.Services;

namespace Bepixplore;

/* Inherit your application services from this class.
 */
public abstract class BepixploreAppService : ApplicationService
{
    protected BepixploreAppService()
    {
        LocalizationResource = typeof(BepixploreResource);
    }
}
