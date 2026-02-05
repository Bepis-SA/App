using Bepixplore.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Bepixplore.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(BepixploreEntityFrameworkCoreModule),
    typeof(BepixploreApplicationContractsModule)
)]
public class BepixploreDbMigratorModule : AbpModule
{
}
