using Bepixplore.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Bepixplore.Permissions;

public class BepixplorePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(BepixplorePermissions.GroupName, L("Permission:Sistema"));

        var metrics = myGroup.AddPermission(BepixplorePermissions.Metrics.Default, L("Permission:Metrics"));
        metrics.AddChild(BepixplorePermissions.Metrics.Admin, L("Permission:Metrics.Admin"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BepixploreResource>(name);
    }
}
