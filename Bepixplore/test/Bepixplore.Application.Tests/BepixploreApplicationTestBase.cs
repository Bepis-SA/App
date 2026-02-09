using Volo.Abp.Modularity;

namespace Bepixplore;

public abstract class BepixploreApplicationTestBase<TStartupModule> : BepixploreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
