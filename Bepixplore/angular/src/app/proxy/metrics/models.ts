
export interface ApiMetricSummaryDto {
  serviceName?: string;
  totalCalls: number;
  successfulCalls: number;
  failedCalls: number;
  averageResponseTime: number;
  successRate: number;
}
