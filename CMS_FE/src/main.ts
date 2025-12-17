import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/APPOINTMENT/app.config';
import { App } from './app/APPOINTMENT/app';

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
