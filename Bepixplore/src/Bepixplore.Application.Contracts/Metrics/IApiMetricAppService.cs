using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Bepixplore.Metrics
{
    public interface IApiMetricAppService : IApplicationService
    {
        Task<List<ApiMetricSummaryDto>> GetSummaryAsync();
    }
}
