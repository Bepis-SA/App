import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { CreateUpdateDestinationDto, DestinationDto } from '../application/contracts/destinations/models';

@Injectable({
  providedIn: 'root',
})
export class FavoriteService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  add = (input: CreateUpdateDestinationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationDto>({
      method: 'POST',
      url: '/api/app/favorite',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationDto[]>({
      method: 'GET',
      url: '/api/app/favorite',
    },
    { apiName: this.apiName,...config });
  

  isFavorite = (name: string, country: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'POST',
      url: '/api/app/favorite/is-favorite',
      params: { name, country },
    },
    { apiName: this.apiName,...config });
  

  remove = (destinationId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/favorite',
      params: { destinationId },
    },
    { apiName: this.apiName,...config });
}