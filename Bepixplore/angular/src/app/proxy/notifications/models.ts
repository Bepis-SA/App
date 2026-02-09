import type { EntityDto } from '@abp/ng.core';
import type { NotificationType } from './notification-type.enum';

export interface NotificationDto extends EntityDto<string> {
  title?: string;
  message?: string;
  isRead: boolean;
  creationTime?: string;
  type?: NotificationType;
}
