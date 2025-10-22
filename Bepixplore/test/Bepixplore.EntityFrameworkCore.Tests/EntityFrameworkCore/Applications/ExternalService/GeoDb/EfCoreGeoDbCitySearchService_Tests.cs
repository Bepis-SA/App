using Bepixplore.ExternalServices.GeoDb;
using Xunit;

namespace Bepixplore.EntityFrameworkCore.Applications;

[Collection(BepixploreTestConsts.CollectionDefinitionName)]
public class EfCoreGeoDbCitySearchService_Tests : GeoDbCitySearchService_Tests<BepixploreEntityFrameworkCoreTestModule>
{

}
