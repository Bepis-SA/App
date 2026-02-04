import type { CityDto } from '../application/contracts/cities/models';

export interface CitySearchRequestDto {
  partialName?: string;
  country?: string;
  minPopulation?: number;
  isPopularFilter: boolean;
}

export interface CitySearchResultDto {
  cities: CityDto[];
}
