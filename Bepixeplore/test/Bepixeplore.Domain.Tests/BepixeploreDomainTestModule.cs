using Volo.Abp.Modularity;

namespace Bepixeplore;

[DependsOn(
    typeof(BepixeploreDomainModule),
    typeof(BepixeploreTestBaseModule)
)]
public class BepixeploreDomainTestModule : AbpModule
{

}
