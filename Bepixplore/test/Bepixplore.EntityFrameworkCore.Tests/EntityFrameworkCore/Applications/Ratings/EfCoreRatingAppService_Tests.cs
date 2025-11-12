using Bepixplore.Application.Tests.Ratings;
using Xunit;

namespace Bepixplore.EntityFrameworkCore.Applications.Ratings
{
    [Collection(BepixploreTestConsts.CollectionDefinitionName)]
    public class EfCoreRatingAppService_Tests : RatingAppService_Tests<BepixploreEntityFrameworkCoreTestModule>
    {
    }
}
