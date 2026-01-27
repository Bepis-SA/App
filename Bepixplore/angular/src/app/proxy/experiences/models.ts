import type { TravelRating } from './travel-rating.enum';
import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateTravelExperienceDto {
  destinationId?: string;
  rating?: TravelRating;
  description?: string;
  travelDate?: string;
}

export interface TravelExperienceDto extends EntityDto<string> {
  destinationId?: string;
  rating?: TravelRating;
  description?: string;
  travelDate?: string;
}
