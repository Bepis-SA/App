import { Routes } from '@angular/router';

export const PUBLIC_PROFILE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./public-profile/public-profile.component').then(c => c.PublicProfileComponent)
  },
];