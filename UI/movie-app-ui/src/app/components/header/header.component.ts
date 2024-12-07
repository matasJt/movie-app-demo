import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatDialog,  MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule} from '@angular/material/button'
import { LoginComponent } from '../login/login.component';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink,MatDialogModule, MatButtonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  isAdmin = false;
  isAuthticated = false;
  constructor(private dialog : MatDialog, private authService: AuthService){}

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
