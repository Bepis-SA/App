import type { CityDto } from '../application/contracts/cities/models';

export interface CitySearchRequestDto {
  partialName?: string;
}

export interface CitySearchResultDto {
  cities: CityDto[];
}
