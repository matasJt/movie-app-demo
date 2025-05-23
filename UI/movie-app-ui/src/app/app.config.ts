import { ApplicationConfig } from '@angular/core';
import { provideRouter, withInMemoryScrolling} from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideToastr } from 'ngx-toastr';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes, withInMemoryScrolling({
        scrollPositionRestoration: 'disabled', 
      })), provideClientHydration(), provideHttpClient(), provideAnimationsAsync(), provideAnimationsAsync(), provideToastr(), 
  ]
};
