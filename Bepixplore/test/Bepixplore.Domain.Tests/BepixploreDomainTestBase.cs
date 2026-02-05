using Volo.Abp.Modularity;

namespace Bepixplore;

/* Inherit from this class for your domain layer tests. */
public abstract class BepixploreDomainTestBase<TStartupModule> : BepixploreTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
