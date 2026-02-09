using Bepixplore.Application.Contracts.Cities;
using Bepixplore.Cities;
using Bepixplore.Destinations;
using Bepixplore.Favorites;
using Bepixplore.Metrics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Linq;


namespace Bepixplore.External.GeoDb
{
    public class GeoDbCitySearchService : ApplicationService, ICitySearchService, ITransientDependency
    {
        private readonly HttpClient _httpClient;
        private readonly IRepository<ApiMetric, Guid> _apiMetricRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly IRepository<Favorite, Guid> _favoriteRepository;
        private readonly IAsyncQueryableExecuter _asyncExecuter;

        private readonly string _baseUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo";
        private readonly string _apiKey = "1b87288382msh04081de1250362fp1acf94jsn6c66e7e31d14";
        private readonly string _host = "wft-geo-db.p.rapidapi.com";

        public GeoDbCitySearchService(
            IAsyncQueryableExecuter asyncExecuter,
            IRepository<Destination, Guid> destinationRepository,
            IRepository<Favorite, Guid> favoriteRepository,
            HttpClient httpClient,
            IRepository<ApiMetric, Guid> apiMetricRepository,
            IGuidGenerator guidGenerator)
        {
            _asyncExecuter = asyncExecuter;
            _destinationRepository = destinationRepository;
            _httpClient = httpClient;
            _apiMetricRepository = apiMetricRepository;
            _guidGenerator = guidGenerator;
            _favoriteRepository = favoriteRepository;
        }

        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            var result = new CitySearchResultDto { Cities = new List<CityDto>() };

            if (request.IsPopularFilter)
            {
                return await GetPopularCitiesFromLocalDbAsync(request);
            }

            if (string.IsNullOrWhiteSpace(request?.PartialName)) return result;

            var stopwatch = Stopwatch.StartNew();
            bool isSuccess = false;
            string errorMessage = null;

            try
            {
                var url = $"{_baseUrl}/cities?namePrefix={Uri.EscapeDataString(request.PartialName)}&limit=5";
                if (!string.IsNullOrWhiteSpace(request.Country)) url += $"&countryIds={Uri.EscapeDataString(request.Country)}";
                if (request.MinPopulation > 0) url += $"&minPopulation={request.MinPopulation}";

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.Add("X-RapidAPI-Key", _apiKey);
                httpRequest.Headers.Add("X-RapidAPI-Host", _host);

                var response = await _httpClient.SendAsync(httpRequest);
                isSuccess = response.IsSuccessStatusCode;

                if (!isSuccess)
                {
                    errorMessage = $"API Error: {(int)response.StatusCode} - {response.ReasonPhrase}";
                }
                else
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var geoDbResponse = await response.Content.ReadFromJsonAsync<GeoDbCitiesResponse>(options);

                    if (geoDbResponse?.Data != null)
                    {
                        foreach (var city in geoDbResponse.Data)
                        {
                            result.Cities.Add(new CityDto
                            {
                                Name = city.Name,
                                Country = city.Country,
                                Population = (uint)city.Population,
                                Latitude = city.Latitude,
                                Longitude = city.Longitude
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                errorMessage = "Excepción: " + ex.Message;
            }
            finally
            {
                stopwatch.Stop();

                try
                {
                    var safeErrorMessage = errorMessage?.Length > 500 ? errorMessage.Substring(0, 497) + "..." : errorMessage;

                    await _apiMetricRepository.InsertAsync(new ApiMetric(
                        _guidGenerator.Create(),
                        "GeoDB",
                        "/v1/geo/cities",
                        isSuccess,
                        (int)stopwatch.ElapsedMilliseconds,
                        safeErrorMessage
                    ));
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        private async Task<CitySearchResultDto> GetPopularCitiesFromLocalDbAsync(CitySearchRequestDto request)
        {
            var destinationsQuery = await _destinationRepository.GetQueryableAsync();
            var favoritesQuery = await _favoriteRepository.GetQueryableAsync();


            var popularIdsQuery = favoritesQuery
                .GroupBy(f => f.DestinationId)
                .Select(g => new {
                    DestinationId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count);

            var query = from fav in popularIdsQuery
                        join dest in destinationsQuery on fav.DestinationId equals dest.Id
                        select dest;

            if (!string.IsNullOrWhiteSpace(request.PartialName))
                query = query.Where(x => x.Name.Contains(request.PartialName));

            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                query = query.Where(x => x.Country.Contains(request.Country));
            }

            if (request.MinPopulation.HasValue && request.MinPopulation > 0)
            {
                query = query.Where(x => x.Population >= request.MinPopulation.Value);
            }

            var popularDestinations = await _asyncExecuter.ToListAsync(query.Take(10));

            return new CitySearchResultDto
            {
                Cities = popularDestinations.Select(city => new CityDto
                {
                    Name = city.Name,
                    Country = city.Country,
                    Population = (uint)city.Population,
                    Latitude = city.Coordinates?.Latitude ?? 0,
                    Longitude = city.Coordinates?.Longitude ?? 0
                }).ToList()
            };
        }

        private class GeoDbCitiesResponse
        {
            [JsonPropertyName("data")]
            public List<GeoDbCity> Data { get; set; }
        }

        private class GeoDbCity
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("country")]
            public string Country { get; set; }

            [JsonPropertyName("latitude")]
            public float Latitude { get; set; }

            [JsonPropertyName("longitude")]
            public float Longitude { get; set; }

            [JsonPropertyName("population")]
            public int Population { get; set; }
        }
    }
}