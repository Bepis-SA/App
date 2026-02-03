using System;
using Volo.Abp.Application.Dtos;

namespace Bepixplore.Metrics
{
    public class ApiMetricSummaryDto
    {
        public string ServiceName { get; set; }
        public int TotalCalls { get; set; }
        public int SuccessfulCalls { get; set; }
        public int FailedCalls { get; set; }
        public double AverageResponseTime { get; set; }
        public double SuccessRate { get; set; } 
    }
}