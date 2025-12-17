import { mergeApplicationConfig, ApplicationConfig } from '@angular/core';
import { provideServerRendering, withRoutes } from '@angular/ssr';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { appConfig } from './app.config';
import { serverRoutes } from './app.routes.server';

const serverConfig: ApplicationConfig = {
  providers: [
    provideServerRendering(withRoutes(serverRoutes)),
    provideClientHydration(withEventReplay())
  ]
};

export const config = mergeApplicationConfig(appConfig, serverConfig);
