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

        //Define your own permissions here. Example:
        //myGroup.AddPermission(BepixplorePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BepixploreResource>(name);
    }
}
