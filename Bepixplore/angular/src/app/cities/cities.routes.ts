import { Routes } from '@angular/router';

export const CITIES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./search-city/search-city.component').then(c => c.SearchCityComponent)
  },
];