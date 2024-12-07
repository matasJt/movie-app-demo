import { Component, OnInit} from '@angular/core';
import { HttpClientModule} from '@angular/common/http';
import { RouterOutlet } from '@angular/router';
import { AsyncPipe} from '@angular/common';
import { HeaderComponent } from "./components/header/header.component";
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HttpClientModule,RouterOutlet,HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
//  private apiUrl = 'http://localhost:5031/api/tags/';
 title = "movie";
//  constructor(private http: HttpClient, 
//              private router: Router ){}
 
//  tags$ = this.getTags()
//  getTags(): Observable<Tag[]> {
//   return this.http.get<Tag[]>(this.apiUrl);
// }
// navigateToLogin():void{
//   this.router.navigate(['/login']);
// }

}
