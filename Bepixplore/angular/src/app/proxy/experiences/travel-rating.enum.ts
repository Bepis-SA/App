import { mapEnumToOptions } from '@abp/ng.core';

export enum TravelRating {
  Positive = 1,
  Neutral = 2,
  Negative = 3,
}

export const travelRatingOptions = mapEnumToOptions(TravelRating);
