using Bepixplore.Destinations;
using Xunit;

namespace Bepixplore.EntityFrameworkCore.Applications.Destinations
{
    [Collection(BepixploreTestConsts.CollectionDefinitionName)]

    public class EfCoreDestinationAppService_Tests : DestinationAppService_Tests<BepixploreEntityFrameworkCoreTestModule>
    {

    }

}