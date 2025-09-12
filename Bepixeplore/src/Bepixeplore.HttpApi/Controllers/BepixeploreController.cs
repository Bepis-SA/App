using Bepixeplore.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Bepixeplore.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class BepixeploreController : AbpControllerBase
{
    protected BepixeploreController()
    {
        LocalizationResource = typeof(BepixeploreResource);
    }
}
