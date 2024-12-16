import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatDialog,  MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule} from '@angular/material/button'
import { LoginComponent } from '../login/login.component';
import { AuthService } from '../../services/auth.service';
import { HostListener } from '@angular/core';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { RegisterComponent } from '../register/register.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink,MatDialogModule, MatButtonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
  animations: [
    trigger('headerAnimation', [
      state('void', style({
        opacity: 0,
        transform: 'translateY(-20px)'
      })),
      transition(':enter', [
        animate('300ms ease-out', style({
          opacity: 1,
          transform: 'translateY(0)'
        }))
      ])
    ]),
    trigger('linkAnimation', [
      state('in', style({
        transform: 'translateX(0)'
      })),
      state('out', style({
        transform: 'translateX(-10px)'
      })),
      transition('* => open', animate('300ms ease-in-out'))
    ])
  ]
})
export class HeaderComponent implements OnInit {
  isAdmin = false;
  isAuthticated = false;
  constructor(private dialog : MatDialog, private authService: AuthService){}
  isShrunk = false;
  private scrollThreshold = 50;

  @HostListener('window:scroll')
  onWindowScroll() {
    const scrollPosition = document.documentElement.scrollTop;
    
    this.isShrunk = scrollPosition > this.scrollThreshold;
  }

  ngOnInit(): void {
      this.authService.isAuthenticated$.subscribe( (status) =>{
        this.isAuthticated = status;
      });
      this.authService.userRole$.subscribe( (role)=>{
          this.isAdmin = role === 'Admin';
      });
  }
  openLogin() {
    this.dialog.open(LoginComponent,{
      height:'auto',
      width:'30%',
      exitAnimationDuration:'300ms'
    });
  }
  openRegister(){
    this.dialog.open(RegisterComponent,{
      height:'auto',
      width:'30%',
      exitAnimationDuration:'300ms'
    });
  }
  logout(){
    this.authService.logout().subscribe({
      next: (response) => {
        console.log('Logged out successfully');
      },
      error: (err) => {
        console.error('Logout failed', err);
      }
    });
  }

}
