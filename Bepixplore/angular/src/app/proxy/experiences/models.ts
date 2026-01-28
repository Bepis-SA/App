import type { TravelRating } from './travel-rating.enum';
import type { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdateTravelExperienceDto {
  destinationId?: string;
  rating?: TravelRating;
  description?: string;
  travelDate?: string;
}

export interface GetTravelExperienceListDto extends PagedAndSortedResultRequestDto {
  destinationId?: string;
  keyword?: string;
  rating?: number;
}

export interface TravelExperienceDto extends EntityDto<string> {
  destinationId?: string;
  rating?: TravelRating;
  description?: string;
  travelDate?: string;
}
