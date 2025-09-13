using Xunit;

namespace Bepixeplore.EntityFrameworkCore;

[CollectionDefinition(BepixeploreTestConsts.CollectionDefinitionName)]
public class BepixeploreEntityFrameworkCoreCollection : ICollectionFixture<BepixeploreEntityFrameworkCoreFixture>
{

}
