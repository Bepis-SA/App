import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { CitySearchRequestDto, CitySearchResultDto } from '../../cities/models';

@Injectable({
  providedIn: 'root',
})
export class GeoDbCitySearchService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  searchCities = (request: CitySearchRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CitySearchResultDto>({
      method: 'POST',
      url: '/api/app/geo-db-city-search/search-cities',
      body: request,
    },
    { apiName: this.apiName,...config });
}