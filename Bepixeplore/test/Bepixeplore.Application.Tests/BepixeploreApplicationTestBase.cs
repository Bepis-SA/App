using Volo.Abp.Modularity;

namespace Bepixeplore;

public abstract class BepixeploreApplicationTestBase<TStartupModule> : BepixeploreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
