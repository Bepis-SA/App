using Bepixplore.Samples;
using Xunit;

namespace Bepixplore.EntityFrameworkCore.Domains;

[Collection(BepixploreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<BepixploreEntityFrameworkCoreTestModule>
{

}
