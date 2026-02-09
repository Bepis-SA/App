import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44377/',
  redirectUri: baseUrl,
  clientId: 'Bepixplore_App',
  responseType: 'code',
  scope: 'offline_access Bepixplore',
  requireHttps: true,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'Bepixplore',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44377',
      rootNamespace: 'Bepixplore',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
