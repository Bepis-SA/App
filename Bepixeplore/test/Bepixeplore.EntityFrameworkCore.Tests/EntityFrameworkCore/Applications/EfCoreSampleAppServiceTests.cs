using Bepixeplore.Samples;
using Xunit;

namespace Bepixeplore.EntityFrameworkCore.Applications;

[Collection(BepixeploreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<BepixeploreEntityFrameworkCoreTestModule>
{

}
