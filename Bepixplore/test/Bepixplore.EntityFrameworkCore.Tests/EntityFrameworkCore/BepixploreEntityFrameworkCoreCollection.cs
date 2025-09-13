using Xunit;

namespace Bepixplore.EntityFrameworkCore;

[CollectionDefinition(BepixploreTestConsts.CollectionDefinitionName)]
public class BepixploreEntityFrameworkCoreCollection : ICollectionFixture<BepixploreEntityFrameworkCoreFixture>
{

}
