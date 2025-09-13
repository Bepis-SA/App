import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44334/',
  redirectUri: baseUrl,
  clientId: 'Bepixeplore_App',
  responseType: 'code',
  scope: 'offline_access Bepixeplore',
  requireHttps: true,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'Bepixeplore',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44334',
      rootNamespace: 'Bepixeplore',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
