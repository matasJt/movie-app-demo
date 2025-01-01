import { Routes, withInMemoryScrolling } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { authGuard } from './auth.guard';
import { MainComponent } from './components/main/main.component';
import { MovieComponent } from './components/movie/movie.component';

export const routes: Routes = [
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path:'main',
        component:MainComponent,
    
    },
    {
        path:'',
        pathMatch:'full',
        redirectTo:'/main',
    },
    {
        path:'movie/:id/:title',
        component: MovieComponent,
        
    }

];
