using Bepixplore.Samples;
using Xunit;

namespace Bepixplore.EntityFrameworkCore.Applications;

[Collection(BepixploreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<BepixploreEntityFrameworkCoreTestModule>
{

}
