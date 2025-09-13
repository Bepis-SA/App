using Bepixplore.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Bepixplore.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class BepixploreController : AbpControllerBase
{
    protected BepixploreController()
    {
        LocalizationResource = typeof(BepixploreResource);
    }
}
