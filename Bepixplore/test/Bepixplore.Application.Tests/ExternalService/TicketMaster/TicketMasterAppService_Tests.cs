using Bepixplore.Events;
using Bepixplore.Metrics;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Shouldly;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using Xunit;

namespace Bepixplore.ExternalService.TicketMaster
{
    public abstract class TicketMasterAppService_Tests<TStartupModule> : BepixploreApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly ITicketMasterAppService _ticketMasterService;

        private class FailingHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw new HttpRequestException("Simulated network error");
            }
        }

        protected TicketMasterAppService_Tests()
        {
            _ticketMasterService = GetRequiredService<ITicketMasterAppService>();
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task GetEventsByCityAsync_Should_Return_Real_Events_And_Map_Detailed_Data()
        {
            // Arrange
            var city = "Los Angeles";

            // Act
            var result = await _ticketMasterService.GetEventsByCityAsync(city);

            // Assert
            result.ShouldNotBeNull();
            if (result.Any())
            {
                var firstEvent = result.First();
                firstEvent.Name.ShouldNotBeNullOrWhiteSpace();
                firstEvent.VenueName.ShouldNotBeNullOrWhiteSpace();
                firstEvent.StartDate.ShouldNotBeNullOrWhiteSpace();
                firstEvent.Url.ShouldStartWith("http");
            }
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task GetEventsByCityAsync_Should_Return_Empty_List_For_Unknown_City()
        {
            // Arrange
            var city = "CityThatDoesNotExist12345";

            // Act
            var result = await _ticketMasterService.GetEventsByCityAsync(city);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task GetEventsByCityAsync_Should_Handle_Network_Error_Gracefully_And_Log_Metric()
        {
            // Arrange
            var failingHttpClient = Substitute.For<IHttpClientFactory>();
            failingHttpClient.CreateClient(Arg.Any<string>()).Returns(new HttpClient(new FailingHandler()));

            var configMock = GetRequiredService<IConfiguration>();
            var apiMetricRepoMock = Substitute.For<IRepository<ApiMetric, Guid>>();
            var guidGenMock = GetRequiredService<IGuidGenerator>();
            var nullLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger<TicketMasterAppService>.Instance;

            var service = new TicketMasterAppService(
                failingHttpClient,
                configMock,
                apiMetricRepoMock,
                guidGenMock,
                nullLogger
            );

            // Act
            var result = await service.GetEventsByCityAsync("London");

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();

            await apiMetricRepoMock.Received(1).InsertAsync(Arg.Is<ApiMetric>(m => !m.IsSuccess));
        }

        [Fact]
        [Trait("Category", "IntegrationTest")]
        public async Task GetEventsByCityAsync_Should_Persist_Metric_On_Success()
        {
            // Arrange
            var city = "New York";
            var apiMetricRepository = GetRequiredService<IRepository<ApiMetric, Guid>>();

            // Act
            await _ticketMasterService.GetEventsByCityAsync(city);

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var metrics = await apiMetricRepository.GetListAsync();
                var lastMetric = metrics
                    .OrderByDescending(m => m.CreationTime)
                    .FirstOrDefault(m => m.ServiceName == "TicketMaster");

                lastMetric.ShouldNotBeNull();
                lastMetric.Endpoint.ShouldBe("/discovery/v2/events");
                lastMetric.ResponseTimeMs.ShouldBeGreaterThan(0);
            });
        }
    }
}
