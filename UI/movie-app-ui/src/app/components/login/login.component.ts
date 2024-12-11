import { Component} from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormControl, AbstractControl} from '@angular/forms';
import { NgClass } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule,NgClass],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent{
  loginForm: FormGroup = new FormGroup({
    username: new FormControl(''),
    password: new FormControl('')
  });
  submitted = false;
  invalidLogin = false;

  constructor(
    private dialog: MatDialogRef<LoginComponent>, 
    private formBuilder: FormBuilder, 
    private authService: AuthService,
    private router: Router,
  ){}
  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      username:[
        '',
        [
          Validators.required,
          Validators.minLength(4),
        ],
      ],
      password:[
        '',
        [
          Validators.required,
          Validators.minLength(6),
          Validators.pattern('^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+\\-={}\\[\\]:;"\'<>,.?/~`|]).{6,}$')
        ],
      ],
    });
  }
  get form():{ [key:string]: AbstractControl}{
    return this.loginForm.controls;
  }
  onSubmit() {
    this.submitted = true;
    if(this.loginForm.valid){
      const {username, password} = this.loginForm.value;
      this.authService.login(username, password).subscribe(
        (response) =>{
          this.dialog.close();
        },
        (error) =>{
          this.invalidLogin = true;
          this.loginForm.reset();
        }
      )
    }
  }
}
