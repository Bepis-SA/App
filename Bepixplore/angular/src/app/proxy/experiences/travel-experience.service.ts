import type { CreateUpdateTravelExperienceDto, TravelExperienceDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TravelExperienceService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateTravelExperienceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TravelExperienceDto>({
      method: 'POST',
      url: '/api/app/travel-experience',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/travel-experience/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (keyword: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TravelExperienceDto[]>({
      method: 'GET',
      url: '/api/app/travel-experience',
      params: { keyword },
    },
    { apiName: this.apiName,...config });
}