using Bepixeplore.Samples;
using Xunit;

namespace Bepixeplore.EntityFrameworkCore.Domains;

[Collection(BepixeploreTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<BepixeploreEntityFrameworkCoreTestModule>
{

}
