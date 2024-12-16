import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule,ReactiveFormsModule,NgClass],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
registerForm: FormGroup = new FormGroup({
    username: new FormControl(''),
    email: new FormControl(''),
    password: new FormControl('')
  });
  submitted = false;
  invalidLogin = false;

  constructor(
    private dialog: MatDialogRef<RegisterComponent>, 
    private formBuilder: FormBuilder, 
    private authService: AuthService,
    private router: Router,
    private notif: ToastrService
  ){}
  ngOnInit(): void {
    this.registerForm = this.formBuilder.group({
      username:[
        '',
        [
          Validators.required,
          Validators.minLength(4),
        ],
      ],
      email:[
        '',
        [
          Validators.required,
          Validators.email,
        ]
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
    return this.registerForm.controls;
  }
  onSubmit() {
    this.submitted = true;
    if(this.registerForm.valid){
      const {username, email, password} = this.registerForm.value;
      this.authService.register(username, email, password).subscribe(
        (response) =>{
          this.notif.success("Success register","",{
            timeOut:1000
          })
          this.dialog.close();
        },
        (error) =>{
          this.invalidLogin = true;
          this.registerForm.reset();
        }
      )
    }
  }
}
