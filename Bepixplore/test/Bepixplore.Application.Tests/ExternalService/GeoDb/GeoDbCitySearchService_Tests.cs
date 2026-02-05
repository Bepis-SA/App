using Bepixplore.Cities;
using Bepixplore.Destinations;
using Bepixplore.External.GeoDb;
using Bepixplore.Favorites;
using Bepixplore.Metrics;
using NSubstitute;
using Shouldly;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Linq;
using Volo.Abp.Modularity;
using Xunit;

namespace Bepixplore.ExternalServices.GeoDb
{
    public abstract class GeoDbCitySearchService_Tests<TStartupModule> : BepixploreApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ICitySearchService _citySearchService;

        private class FailingHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw new HttpRequestException("Simulated network error");
            }
        }
        protected GeoDbCitySearchService_Tests()
        {
            _citySearchService = GetRequiredService<ICitySearchService>();
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_Should_Return_Real_Cities_And_Map_Correctly()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "Córdo" };

            // Act
            var result = await _citySearchService.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldNotBeEmpty();

            // Assert (Mapping Dto)
            var cordoba = result.Cities.FirstOrDefault(c => c.Name == "Córdoba");
            cordoba.ShouldNotBeNull();
            cordoba.Country.ShouldBe("Argentina");
            cordoba.Population.ShouldBeGreaterThan(0u);
            cordoba.Latitude.ShouldNotBe(0f);
            cordoba.Longitude.ShouldNotBe(0f);
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_Should_Return_Empty_List_For_No_Match()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "zzxxyyqq" };

            // Act
            var result = await _citySearchService.SearchCitiesAsync(request);

            //Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldBeEmpty();
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_Should_Return_Empty_List_For_Invalid_Input()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "" };

            // Act
            var result = await _citySearchService.SearchCitiesAsync(request);

            //Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldBeEmpty();
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_Should_Handle_Network_Error_Gracefully()
        {
            // Arrange
            var failingHttpClient = new HttpClient(new FailingHandler());

            var asyncExecuterMock = Substitute.For<IAsyncQueryableExecuter>();
            var destinationRepoMock = Substitute.For<IRepository<Destination, Guid>>();
            var favoriteRepoMock = Substitute.For<IRepository<Favorite, Guid>>();

            var apiMetricRepositoryMock = Substitute.For<IRepository<ApiMetric, Guid>>();
            var guidGeneratorMock = Substitute.For<IGuidGenerator>();

            var service = new GeoDbCitySearchService(
                asyncExecuterMock,
                destinationRepoMock,
                favoriteRepoMock,
                failingHttpClient,
                apiMetricRepositoryMock,
                guidGeneratorMock
            );

            var request = new CitySearchRequestDto { PartialName = "Cor", IsPopularFilter = false };

            // Act
            var result = await service.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldBeEmpty();

            await apiMetricRepositoryMock.Received(1).InsertAsync(Arg.Is<ApiMetric>(m => !m.IsSuccess));
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_Should_Persist_Metric_In_Database()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "Cordo" };
            var apiMetricRepository = GetRequiredService<IRepository<ApiMetric, Guid>>();

            // Act
            await _citySearchService.SearchCitiesAsync(request);

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var metrics = await apiMetricRepository.GetListAsync();

                metrics.ShouldNotBeEmpty();
                var lastMetric = metrics.FirstOrDefault(m => m.ServiceName == "GeoDB");

                lastMetric.ShouldNotBeNull();
                lastMetric.IsSuccess.ShouldBeTrue();
                lastMetric.ResponseTimeMs.ShouldBeGreaterThan(0);
                lastMetric.Endpoint.ShouldBe("/v1/geo/cities");
            });
        }

        [Fact]
        public async Task SearchCitiesAsync_Should_Apply_Country_Filters()
        {
            var request = new CitySearchRequestDto { PartialName = "San", Country = "US" };
            var result = await _citySearchService.SearchCitiesAsync(request);

            foreach (var city in result.Cities)
            {
                city.Country.ShouldContain("United States");
            }
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task SearchCitiesAsync_Should_Map_Detailed_Technical_Data()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "Tokyo" };

            // Act
            var result = await _citySearchService.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldNotBeEmpty();

            var tokyo = result.Cities.FirstOrDefault(c => c.Name.Contains("Toky", StringComparison.OrdinalIgnoreCase));

            if (tokyo == null) tokyo = result.Cities.First();

            tokyo.ShouldNotBeNull();
            tokyo.Country.ShouldNotBeNullOrWhiteSpace();
            tokyo.Latitude.ShouldBeInRange(20.0f, 45.0f);
            tokyo.Longitude.ShouldBeInRange(120.0f, 150.0f);
        }

    }
}