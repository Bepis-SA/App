import { Routes } from '@angular/router';
import { eLayoutType } from '@abp/ng.core';
import { authGuard } from '@abp/ng.core';

export const CITIES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./search-city/search-city.component').then(c => c.SearchCityComponent)
  },
];