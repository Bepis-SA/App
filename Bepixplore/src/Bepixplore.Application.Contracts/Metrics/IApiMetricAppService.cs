using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Bepixplore.Metrics
{
    public interface IApiMetricAppService : IApplicationService
    {
        Task<List<ApiMetricSummaryDto>> GetSummaryAsync();
    }
}
