using Bepixeplore.Localization;
using Volo.Abp.Application.Services;

namespace Bepixeplore;

/* Inherit your application services from this class.
 */
public abstract class BepixeploreAppService : ApplicationService
{
    protected BepixeploreAppService()
    {
        LocalizationResource = typeof(BepixeploreResource);
    }
}
