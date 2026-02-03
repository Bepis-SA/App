using Bepixplore.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Bepixplore.Permissions;

public class BepixplorePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(BepixplorePermissions.GroupName);

        var metricsPermission = myGroup.AddPermission(
            BepixplorePermissions.Metrics.Default,
            L("Permission:Metrics"));

        metricsPermission.AddChild(
            BepixplorePermissions.Metrics.Admin,
            L("Permission:Metrics.Admin"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BepixploreResource>(name);
    }
}
