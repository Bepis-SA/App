import { authGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

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
    path: 'favorites',
    loadComponent: () => import('./favorites/favorites.component').then(m => m.FavoritesComponent),
    canActivate: [authGuard],
  },
  {
    path: 'cities/details/:id',
    loadComponent: () => import('./cities/city-detail/city-detail.component').then(m => m.CityDetailComponent),
  },
  {
    path: 'cities/details',
    loadComponent: () => import('./cities/city-detail/city-detail.component').then(m => m.CityDetailComponent),
  },
  {
    path: 'favorites/details/:id',
    loadComponent: () => import('./cities/city-detail/city-detail.component').then(m => m.CityDetailComponent),
  },
  {
    path: 'public-profile',
    loadChildren: () => import('./public-profile/public-profile.routes').then(c => c.PUBLIC_PROFILE_ROUTES),
    canActivate: [authGuard],
  },
];