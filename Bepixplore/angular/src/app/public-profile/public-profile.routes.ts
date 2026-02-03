import { Routes } from '@angular/router';
import { eLayoutType } from '@abp/ng.core';
import { authGuard } from '@abp/ng.core';

export const PUBLIC_PROFILE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./public-profile/public-profile.component').then(c => c.PublicProfileComponent)
  },
];