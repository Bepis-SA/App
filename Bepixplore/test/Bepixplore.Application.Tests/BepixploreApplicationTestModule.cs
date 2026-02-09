using Volo.Abp.Modularity;

namespace Bepixplore;

[DependsOn(
    typeof(BepixploreApplicationModule),
    typeof(BepixploreDomainTestModule)
)]
public class BepixploreApplicationTestModule : AbpModule
{

}
