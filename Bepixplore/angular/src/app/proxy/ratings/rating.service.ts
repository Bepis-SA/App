import type { CreateUpdateRatingDto, RatingDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class RatingService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateRatingDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RatingDto>({
      method: 'POST',
      url: '/api/app/rating',
      params: { destinationId: input.destinationId, score: input.score, comment: input.comment },
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/rating/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RatingDto>({
      method: 'GET',
      url: `/api/app/rating/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<RatingDto>>({
      method: 'GET',
      url: '/api/app/rating',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateRatingDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RatingDto>({
      method: 'PUT',
      url: `/api/app/rating/${id}`,
      params: { destinationId: input.destinationId, score: input.score, comment: input.comment },
    },
    { apiName: this.apiName,...config });
}