export enum NotificationChannel {
  Email = 0,
  Screen = 1,
  Both = 2 
}

export enum NotificationFrequency {
  Immediate = 0,
  WeeklySummary = 1
}

export interface UserProfileDto {
  userName: string;
  email: string;
  name: string;
  surname: string;
  phoneNumber: string;
  isExternal: boolean;
  hasPassword: boolean;
  concurrencyStamp?: string;
  extraProperties: {
    [key: string]: any; 
  };
}