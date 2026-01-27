using Bepixplore.Application.Contracts.Cities;
using Bepixplore.Cities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Bepixplore.External.GeoDb
{
    public class GeoDbCitySearchService : ICitySearchService, ITransientDependency
    {
        private readonly HttpClient _httpClient;

        private readonly string _baseUrl = "https://wft-geo-db.p.rapidapi.com/v1/geo";
        private readonly string _apiKey = "1b87288382msh04081de1250362fp1acf94jsn6c66e7e31d14";
        private readonly string _host = "wft-geo-db.p.rapidapi.com";

        public GeoDbCitySearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            var result = new CitySearchResultDto
            {
                Cities = new List<CityDto>()
            };

            if (string.IsNullOrWhiteSpace(request?.PartialName))
                return result;

            var url = $"{_baseUrl}/cities?namePrefix={Uri.EscapeDataString(request.PartialName)}&limit=5";
            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                url += $"&countryIds={Uri.EscapeDataString(request.Country)}";
            }
            if (request.MinPopulation > 0)
            {
                url += $"&minPopulation={request.MinPopulation}";
            }
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequest.Headers.Add("X-RapidAPI-Key", _apiKey);
            httpRequest.Headers.Add("X-RapidAPI-Host", _host);

            try
            {
                var response = await _httpClient.SendAsync(httpRequest);

                if (!response.IsSuccessStatusCode)
                {
                    return result;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
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
            catch (Exception ex)
            {
            }
            return result;
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