import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';
import { CityDetailComponent } from './city-detail/city-detail';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
  {
    path: 'cities',
    loadChildren: () => import('./cities/cities.routes').then(m => m.CITIES_ROUTES),
    canActivate: [authGuard],
  },
  {
    path: 'destinations', // 👈 Esta es la ruta para "Mis Destinos"
    loadComponent: () => import('./destinations/destinations').then(m => m.Destinations),
  },
  // En APP_ROUTES
  {
    path: 'destinations/details/:id', // 👈 El :id es OBLIGATORIO
    loadComponent: () => import('./city-detail/city-detail').then(m => m.CityDetailComponent),
  },

  {
    path: 'destinations/details',
    component: CityDetailComponent,
  },


  {
    path: 'destinations/details',
    loadComponent: () =>
      import('./city-detail/city-detail')
        .then(m => m.CityDetailComponent), 
  },

];
