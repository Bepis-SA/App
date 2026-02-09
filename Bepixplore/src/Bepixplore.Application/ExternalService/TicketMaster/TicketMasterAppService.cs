using Bepixplore;
using Bepixplore.Events;
using Bepixplore.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

public class TicketMasterAppService : BepixploreAppService, ITicketMasterAppService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IRepository<ApiMetric, Guid> _apiMetricRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ILogger<TicketMasterAppService> _logger;

    public TicketMasterAppService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IRepository<ApiMetric, Guid> apiMetricRepository, // <--- Inyectar esto
    IGuidGenerator guidGenerator, ILogger<TicketMasterAppService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _apiMetricRepository = apiMetricRepository;
        _guidGenerator = guidGenerator;
        _logger = logger;
    }

    public async Task<List<TicketMasterEventDto>> GetEventsByCityAsync(string cityName)
    {
        var apiKey = _configuration["TicketMaster:ApiKey"] ?? "pis6jNIILZnLRYvDksINQffc2FYx3G8C";
        var baseUrl = "https://app.ticketmaster.com/discovery/v2/events.json";

        var client = _httpClientFactory.CreateClient();
        var events = new List<TicketMasterEventDto>();

        var stopwatch = Stopwatch.StartNew();
        bool isSuccess = false;
        string errorMessage = null;

        try
        {
            string url = $"{baseUrl}?apikey={apiKey}&city={Uri.EscapeDataString(cityName)}";

            var response = await client.GetAsync(url);
            isSuccess = response.IsSuccessStatusCode;

            if (isSuccess)
            {
                string jsonResult = await response.Content.ReadAsStringAsync();
                events = ParseTicketMasterResponse(jsonResult);
            }
            else
            {
                errorMessage = $"TicketMaster API Error: {(int)response.StatusCode} - {response.ReasonPhrase}";
                Logger.LogWarning(errorMessage);
            }
        }
        catch (Exception ex)
        {
            isSuccess = false;
            errorMessage = "Excepción en TicketMaster: " + ex.Message;
            _logger.LogError(ex, errorMessage);
        }
        finally
        {
            stopwatch.Stop();

            try
            {
                var safeErrorMessage = errorMessage?.Length > 500
                    ? errorMessage.Substring(0, 497) + "..."
                    : errorMessage;

                await _apiMetricRepository.InsertAsync(new ApiMetric(
                    _guidGenerator.Create(),
                    "TicketMaster",
                    "/discovery/v2/events",
                    isSuccess,
                    (int)stopwatch.ElapsedMilliseconds,
                    safeErrorMessage
                ));
            }
            catch (Exception)
            {
            }
        }

        return events;
    }

    private List<TicketMasterEventDto> ParseTicketMasterResponse(string json)
    {
        var events = new List<TicketMasterEventDto>();
        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            if (doc.RootElement.TryGetProperty("_embedded", out var embedded) &&
                embedded.TryGetProperty("events", out var eventsList))
            {
                foreach (var item in eventsList.EnumerateArray())
                {
                    string venueName = "Lugar no definido";

                    if (item.TryGetProperty("_embedded", out var eventEmbedded) &&
                        eventEmbedded.TryGetProperty("venues", out var venues) &&
                        venues.GetArrayLength() > 0)
                    {
                        venueName = venues[0].GetProperty("name").GetString() ?? "Lugar no definido";
                    }

                    events.Add(new TicketMasterEventDto
                    {
                        Name = item.GetProperty("name").GetString() ?? "Sin nombre",
                        Url = item.GetProperty("url").GetString() ?? "",
                        StartDate = item.GetProperty("dates")
                                        .GetProperty("start")
                                        .GetProperty("localDate").GetString() ?? "",
                        VenueName = venueName
                    });
                }
            }
        }
        return events;
    }
}