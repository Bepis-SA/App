namespace Bepixplore.Permissions;

public static class BepixplorePermissions
{
    public const string GroupName = "Bepixplore";

    public static class Metrics
    {
        public const string Default = GroupName + ".Metrics";
        public const string Admin = Default + ".Admin";
    }

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
}
