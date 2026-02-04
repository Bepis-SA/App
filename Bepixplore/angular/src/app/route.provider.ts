import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
    {
      path: '/',
      name: '::Menu:Home',
      iconClass: 'fas fa-home',
      order: 1,
      layout: eLayoutType.application,
    },
    {
      path: '/cities',
      name: '::Menu:Cities',
      iconClass: 'fas fa-search-location',
      order: 2,
      layout: eLayoutType.application,
    },
    {
      path: '/favorites',
      name: 'Mis Destinos',
      iconClass: 'fas fa-heart',
      order: 3,
      layout: eLayoutType.application,
    },
    {
      path: '/public-profile',
      name: '::Menu:PublicProfile',
      iconClass: 'fas fa-users',
      order: 4,
      layout: eLayoutType.application,
    },
  ]);
}
