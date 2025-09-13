using Volo.Abp.Modularity;

namespace Bepixplore;

[DependsOn(
    typeof(BepixploreDomainModule),
    typeof(BepixploreTestBaseModule)
)]
public class BepixploreDomainTestModule : AbpModule
{

}
