import { mapEnumToOptions } from '@abp/ng.core';

export enum NotificationType {
  DestinationUpdate = 1,
  System = 2,
}

export const notificationTypeOptions = mapEnumToOptions(NotificationType);
