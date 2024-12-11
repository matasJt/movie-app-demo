import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { authGuard } from './auth.guard';
import { MainComponent } from './components/main/main.component';

export const routes: Routes = [
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'admin',
        component: HomeComponent,
        canActivate: [authGuard]
    },
    {
        path:'main',
        component:MainComponent
    }

];
