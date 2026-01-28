import type { CreateUpdateTravelExperienceDto, GetTravelExperienceListDto, TravelExperienceDto } from './models';
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
  

  getList = (input: GetTravelExperienceListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TravelExperienceDto[]>({
      method: 'GET',
      url: '/api/app/travel-experience',
      params: { destinationId: input.destinationId, keyword: input.keyword, rating: input.rating, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateTravelExperienceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TravelExperienceDto>({
      method: 'PUT',
      url: `/api/app/travel-experience/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}