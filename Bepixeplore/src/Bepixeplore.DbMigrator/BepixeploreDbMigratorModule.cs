using Bepixeplore.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Bepixeplore.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(BepixeploreEntityFrameworkCoreModule),
    typeof(BepixeploreApplicationContractsModule)
)]
public class BepixeploreDbMigratorModule : AbpModule
{
}
