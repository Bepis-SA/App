import type { PublicUserProfileDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  deleteMyAccount = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/user/my-account',
    },
    { apiName: this.apiName,...config });
  

  getPublicProfile = (userName: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PublicUserProfileDto>({
      method: 'GET',
      url: '/api/app/user/public-profile',
      params: { userName },
    },
    { apiName: this.apiName,...config });
}