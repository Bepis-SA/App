using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Bepixplore.Events
{
    public interface ITicketMasterAppService : IApplicationService
    {
        Task<List<TicketMasterEventDto>> GetEventsByCityAsync(string cityName);
    }
}
