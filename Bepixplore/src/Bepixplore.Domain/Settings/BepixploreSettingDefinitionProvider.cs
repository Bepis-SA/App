using Volo.Abp.Settings;

namespace Bepixplore.Settings;

public static class AccountSettings
{
    public const string IsSelfRegistrationEnabled = "Abp.Account.IsSelfRegistrationEnabled";
}

public class BepixploreSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(BepixploreSettings.MySetting1));

        var registrationSetting = context.GetOrNull(AccountSettings.IsSelfRegistrationEnabled);

        if (registrationSetting != null)
        {
            registrationSetting.DefaultValue = "false";
        }
    }
}