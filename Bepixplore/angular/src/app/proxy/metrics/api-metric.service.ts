import type { ApiMetricSummaryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ApiMetricService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getSummary = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiMetricSummaryDto[]>({
      method: 'GET',
      url: '/api/app/api-metric/summary',
    },
    { apiName: this.apiName,...config });
}