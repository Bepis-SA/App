import type { AuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdateRatingDto {
  destinationId: string;
  score: number;
  comment?: string;
}

export interface RatingDto extends AuditedEntityDto<string> {
  userId?: string;
  destinationId?: string;
  score: number;
  comment?: string;
}
