using Bepixplore.Application.Contracts.Cities;
using Bepixplore.Application.Contracts.Destinations;
using Bepixplore.Cities;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace Bepixplore.Destinations
{
    public abstract class DestinationAppService_Tests<TStartupModule> : BepixploreApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IDestinationAppService _destinationAppService;
        private readonly ICitySearchService _citySearchService;
        private readonly IRepository<Destination, Guid> _destinationRepository;

        protected DestinationAppService_Tests()
        {
            _destinationAppService = GetRequiredService<IDestinationAppService>();
            _destinationRepository = GetRequiredService<IRepository<Destination, Guid>>();
            _citySearchService = Substitute.For<ICitySearchService>();
        }

        /* ----- TESTS FOR DESTINATION CREATION AND VALIDATION ----- */

        [Fact]
        public async Task CreateAsync_Should_Create_Destination_Successfully()
        {
            //Arrange
            var input = new CreateUpdateDestinationDto
            {
                Name = "Test Destination",
                Country = "Test Country",
                City = "Test City",
                Population = 100000,
                Photo = "test_photo.jpg",
                UpdateDate = DateTime.UtcNow,
                Coordinates = new CoordinatesDto { Latitude = 40.7128f, Longitude = -74.0060f }
            };

            //Act
            var result = await _destinationAppService.CreateAsync(input);

            //Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.Name.ShouldBe(input.Name);
            result.Country.ShouldBe(input.Country);
            result.City.ShouldBe(input.City);
            result.Population.ShouldBe(input.Population);
            result.Photo.ShouldBe(input.Photo);
            result.UpdateDate.ShouldBe(input.UpdateDate);
            result.Coordinates.Latitude.ShouldBe(input.Coordinates.Latitude);
            result.Coordinates.Longitude.ShouldBe(input.Coordinates.Longitude);

            var savedDestination = await _destinationRepository.GetAsync(result.Id);
            savedDestination.ShouldNotBeNull();
            savedDestination.Name.ShouldBe(input.Name);
            savedDestination.Coordinates.Latitude.ShouldBe(input.Coordinates.Latitude);
        }

        [Fact]
        public async Task CreateAsync_Should_Not_Allow_Invalid_Values()
        {
            //Act
            var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                await _destinationAppService.CreateAsync(
                    new CreateUpdateDestinationDto
                    {
                        Name = "",
                        Country = "Test Country",
                        City = "Test City",
                        Population = 100000,
                        Photo = "test_photo.jpg",
                        UpdateDate = DateTime.UtcNow,
                        Coordinates = new CoordinatesDto { Latitude = 100.0f, Longitude = -74.0060f }
                    }
                    );
            });

            //Assert
            exception.ValidationErrors.ShouldContain(err => err.MemberNames.Any(mem => mem == "Name"));
            exception.ValidationErrors.ShouldContain(err => err.MemberNames.Contains("Latitude"));
        }

        [Fact]
        public async Task CreateAsync_Should_Respond_Expectedly_With_Valid_Input()
        {
            //Arrange
            var input = new CreateUpdateDestinationDto
            {
                Name = "Valid Destination",
                Country = "Valid Country",
                City = "Valid City",
                Population = 200000,
                Photo = "valid_photo.jpg",
                UpdateDate = DateTime.UtcNow,
                Coordinates = new CoordinatesDto { Latitude = 48.8566f, Longitude = 2.3522f }
            };

            //Act
            var result = await _destinationAppService.CreateAsync(input);

            //Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
            result.CreationTime.ShouldNotBe(default);
        }


        /* ----- TEST TO VERIFY CITY SEARCH FUNCTIONALITY ----- */

        [Fact]
        public async Task SearchCitiesAsync_Should_Return_Cities_When_Service_Finds_Results()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "Cord" };
            var mockResult = new CitySearchResultDto
            {
                Cities = new List<CityDto>
                {
                    new CityDto { Name = "Cordoba", Country = "Argentina" },
                    new CityDto { Name = "Cordoba", Country = "Spain" }
                }
            };
            var destinationRepositoryMock = Substitute.For<IRepository<Destination, Guid>>();
            var citySearchServiceMock = Substitute.For<ICitySearchService>();
            citySearchServiceMock.SearchCitiesAsync(request).Returns(mockResult);
            var destinationAppService = new DestinationAppService(destinationRepositoryMock, citySearchServiceMock);

            // Act
            var result = await destinationAppService.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldNotBeEmpty();
            result.Cities.Count.ShouldBe(2);
        }

        [Fact]
        public async Task SearchCitiesAsync_Should_Return_Empty_When_Service_Finds_No_Results()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "NonExistentCity" };
            var mockResult = new CitySearchResultDto { Cities = new List<CityDto>() };
            var destinationRepositoryMock = Substitute.For<IRepository<Destination, Guid>>();
            var citySearchServiceMock = Substitute.For<ICitySearchService>();
            citySearchServiceMock.SearchCitiesAsync(request).Returns(mockResult);
            var destinationAppService = new DestinationAppService(destinationRepositoryMock, citySearchServiceMock);

            // Act
            var result = await destinationAppService.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchCitiesAsync_Should_Return_Empty_When_Input_Is_Invalid()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "" };
            var mockResult = new CitySearchResultDto { Cities = new List<CityDto>() };
            var destinationRepositoryMock = Substitute.For<IRepository<Destination, Guid>>();
            var citySearchServiceMock = Substitute.For<ICitySearchService>();
            citySearchServiceMock.SearchCitiesAsync(request).Returns(mockResult);
            var destinationAppService = new DestinationAppService(destinationRepositoryMock, citySearchServiceMock);

            // Act
            var result = await destinationAppService.SearchCitiesAsync(request);

            // Assert
            result.ShouldNotBeNull();
            result.Cities.ShouldBeEmpty();
        }

        [Fact]
        public async Task SearchCitiesAsync_Should_Throw_Exception_When_Service_Throws_Exception()
        {
            // Arrange
            var request = new CitySearchRequestDto { PartialName = "Test" };
            var destinationRepositoryMock = Substitute.For<IRepository<Destination, Guid>>();
            var citySearchServiceMock = Substitute.For<ICitySearchService>();
            citySearchServiceMock.When(x => x.SearchCitiesAsync(request)).Do(x => { throw new HttpRequestException("Simulated API error"); });
            var destinationAppService = new DestinationAppService(destinationRepositoryMock, citySearchServiceMock);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await destinationAppService.SearchCitiesAsync(request);
            });

            exception.Message.ShouldBe("Simulated API error");
        }
    }
}

