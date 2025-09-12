using Volo.Abp.Modularity;

namespace Bepixeplore;

/* Inherit from this class for your domain layer tests. */
public abstract class BepixeploreDomainTestBase<TStartupModule> : BepixeploreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
