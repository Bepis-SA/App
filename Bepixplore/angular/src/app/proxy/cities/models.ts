import type { CityDto } from '../application/contracts/cities/models';

export interface CitySearchRequestDto {
  partialName?: string;
  country?: string;
  minPopulation?: number;
}

export interface CitySearchResultDto {
  cities: CityDto[];
}
