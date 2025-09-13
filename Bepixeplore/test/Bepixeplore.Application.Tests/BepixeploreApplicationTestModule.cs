using Volo.Abp.Modularity;

namespace Bepixeplore;

[DependsOn(
    typeof(BepixeploreApplicationModule),
    typeof(BepixeploreDomainTestModule)
)]
public class BepixeploreApplicationTestModule : AbpModule
{

}
