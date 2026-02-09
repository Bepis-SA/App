using Bepixplore.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Bepixplore.Metrics
{
    [Authorize(BepixplorePermissions.Metrics.Admin)]
    public class ApiMetricAppService : ApplicationService, IApiMetricAppService
    {
        private readonly IRepository<ApiMetric, Guid> _apiMetricRepository;

        public ApiMetricAppService(IRepository<ApiMetric, Guid> apiMetricRepository)
        {
            _apiMetricRepository = apiMetricRepository;
        }

        public async Task<List<ApiMetricSummaryDto>> GetSummaryAsync()
        {
            var metrics = await _apiMetricRepository.GetListAsync();

            var summary = metrics
                .GroupBy(m => m.ServiceName)
                .Select(g => new ApiMetricSummaryDto
                {
                    ServiceName = g.Key,
                    TotalCalls = g.Count(),
                    SuccessfulCalls = g.Count(x => x.IsSuccess),
                    FailedCalls = g.Count(x => !x.IsSuccess),
                    AverageResponseTime = g.Any() ? g.Average(x => x.ResponseTimeMs) : 0,
                    SuccessRate = g.Any() ? ((double)g.Count(x => x.IsSuccess) / g.Count()) * 100 : 0
                })
                .ToList();

            return summary;
        }
    }
}