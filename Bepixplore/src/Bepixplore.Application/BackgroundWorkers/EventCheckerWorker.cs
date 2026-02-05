using System;
using System.Linq;
using System.Threading.Tasks;
using Bepixplore.Destinations;
using Bepixplore.Events;
using Bepixplore.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;

namespace Bepixplore.BackgroundWorkers;

public class EventCheckerWorker : AsyncPeriodicBackgroundWorkerBase
{
    public EventCheckerWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory)
        : base(timer, serviceScopeFactory)
    {   //ACA CAMBIAS CADA CUANTO BUSCA LOS EVENTOS, EL TP DICE EJ:24HRS
        Timer.Period = 6000;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        Logger.LogInformation("--- Iniciando EventCheckerWorker: Buscando novedades en TicketMaster ---");

        var destinationRepository = workerContext.ServiceProvider.GetRequiredService<IRepository<Destination, Guid>>();
        var ticketMasterService = workerContext.ServiceProvider.GetRequiredService<ITicketMasterAppService>();
        var notificationAppService = workerContext.ServiceProvider.GetRequiredService<INotificationAppService>();

        try
        {
            var destinations = await destinationRepository.GetListAsync();
            var cities = destinations.Select(d => d.City).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();

            foreach (var city in cities)
            {
                Logger.LogInformation($"Consultando eventos para: {city}");

                var events = await ticketMasterService.GetEventsByCityAsync(city);

                if (events != null && events.Any())
                {
                    var latestEvent = events.First();

                    await notificationAppService.NotifyNewEventAsync(city, latestEvent.Name, latestEvent.VenueName, latestEvent.StartDate);

                    Logger.LogInformation($"Evento detectado en {city}: {latestEvent.Name}. Notificación enviada.");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error crítico en EventCheckerWorker: {ex.Message}");
        }

        Logger.LogInformation("--- Finalizada la ejecución de EventCheckerWorker ---");
    }
}