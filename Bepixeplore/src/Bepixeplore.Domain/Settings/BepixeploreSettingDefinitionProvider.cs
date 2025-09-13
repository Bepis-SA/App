using Volo.Abp.Settings;

namespace Bepixeplore.Settings;

public class BepixeploreSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(BepixeploreSettings.MySetting1));
    }
}
