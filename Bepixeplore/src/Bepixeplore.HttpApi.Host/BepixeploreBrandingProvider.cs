using Microsoft.Extensions.Localization;
using Bepixeplore.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Bepixeplore;

[Dependency(ReplaceServices = true)]
public class BepixeploreBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<BepixeploreResource> _localizer;

    public BepixeploreBrandingProvider(IStringLocalizer<BepixeploreResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
