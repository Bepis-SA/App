using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace Bepixplore.Metrics;

public abstract class ApiMetricAppService_Tests<TStartupModule> : BepixploreApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IApiMetricAppService _apiMetricAppService;
    private readonly IRepository<ApiMetric, Guid> _apiMetricRepository;

    protected ApiMetricAppService_Tests()
    {
        _apiMetricAppService = GetRequiredService<IApiMetricAppService>();
        _apiMetricRepository = GetRequiredService<IRepository<ApiMetric, Guid>>();
    }

    [Fact]
    public async Task GetSummaryAsync_Should_Calculate_Statistics_Correctly()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () =>
        {
            await _apiMetricRepository.InsertAsync(new ApiMetric(Guid.NewGuid(), "GeoDB", "/cities", true, 100, null));
            await _apiMetricRepository.InsertAsync(new ApiMetric(Guid.NewGuid(), "GeoDB", "/cities", true, 200, null));

            await _apiMetricRepository.InsertAsync(new ApiMetric(Guid.NewGuid(), "GeoDB", "/cities", false, 50, "Error"));
            await _apiMetricRepository.InsertAsync(new ApiMetric(Guid.NewGuid(), "GeoDB", "/cities", false, 50, "Error"));
        });

        // Act
        var summary = await _apiMetricAppService.GetSummaryAsync();

        // Assert
        summary.ShouldNotBeNull();
        var geoDbStats = summary.FirstOrDefault(x => x.ServiceName == "GeoDB");

        geoDbStats.ShouldNotBeNull();
        geoDbStats.TotalCalls.ShouldBe(4);
        geoDbStats.SuccessfulCalls.ShouldBe(2);
        geoDbStats.FailedCalls.ShouldBe(2);
        geoDbStats.AverageResponseTime.ShouldBe(100);
        geoDbStats.SuccessRate.ShouldBe(50);
    }
}