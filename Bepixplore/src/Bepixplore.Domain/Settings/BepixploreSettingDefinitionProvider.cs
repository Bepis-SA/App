using Volo.Abp.Settings;

namespace Bepixplore.Settings;

public class BepixploreSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(BepixploreSettings.MySetting1));
    }
}
