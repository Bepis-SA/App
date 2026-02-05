using Microsoft.Extensions.Localization;
using Bepixplore.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Bepixplore;

[Dependency(ReplaceServices = true)]
public class BepixploreBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<BepixploreResource> _localizer;

    public BepixploreBrandingProvider(IStringLocalizer<BepixploreResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
