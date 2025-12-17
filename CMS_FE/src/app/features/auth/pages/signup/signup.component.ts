import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="auth-container">
      <mat-card class="auth-card">
        <mat-card-header>
          <mat-card-title>Create Account</mat-card-title>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="signupForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Full Name</mat-label>
              <input matInput formControlName="name" required>
              <mat-error *ngIf="signupForm.get('name')?.hasError('required')">
                Name is required
              </mat-error>
              <mat-error *ngIf="signupForm.get('name')?.hasError('minlength')">
                Name must be at least 2 characters
              </mat-error>
              <mat-error *ngIf="signupForm.get('name')?.hasError('pattern')">
                Name can only contain letters and spaces
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" required>
              <mat-error *ngIf="signupForm.get('email')?.hasError('required')">
                Email is required
              </mat-error>
              <mat-error *ngIf="signupForm.get('email')?.hasError('email')">
                Please enter a valid email
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Phone Number</mat-label>
              <input matInput formControlName="phoneNumber" placeholder="+1234567890" required>
              <mat-error *ngIf="signupForm.get('phoneNumber')?.hasError('required')">
                Phone number is required
              </mat-error>
              <mat-error *ngIf="signupForm.get('phoneNumber')?.hasError('pattern')">
                Please enter a valid phone number
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Password</mat-label>
              <input matInput type="password" formControlName="password" required>
              <mat-error *ngIf="signupForm.get('password')?.hasError('required')">
                Password is required
              </mat-error>
              <mat-error *ngIf="signupForm.get('password')?.hasError('minlength')">
                Password must be at least 8 characters
              </mat-error>
              <mat-error *ngIf="signupForm.get('password')?.hasError('pattern')">
                Password must contain uppercase, lowercase, and number
              </mat-error>
            </mat-form-field>

            <div class="error-message" *ngIf="errorMessage">
              {{ errorMessage }}
            </div>

            <pre *ngIf="errorDetails" style="white-space:pre-wrap;background:#f5f5f5;padding:8px;border-radius:4px;margin-top:8px;">{{ errorDetails | json }}</pre>

            <div class="success-message" *ngIf="successMessage">
              {{ successMessage }}
            </div>

            <button mat-raised-button color="primary" type="submit" 
                    [disabled]="signupForm.invalid || (loading$ | async)" class="full-width">
              <mat-spinner diameter="20" *ngIf="loading$ | async"></mat-spinner>
              <span *ngIf="!(loading$ | async)">Sign Up</span>
            </button>
          </form>
        </mat-card-content>

        <mat-card-actions>
          <div class="auth-links">
            <a routerLink="/auth/login">Already have an account? Login</a>
          </div>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styles: [`
    .auth-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      padding: 1rem;
    }
    
    .auth-card {
      width: 100%;
      max-width: 400px;
    }
    
    .full-width {
      width: 100%;
      margin-bottom: 1rem;
    }
    
    .error-message {
      color: #f44336;
      margin-bottom: 1rem;
      text-align: center;
    }
    
    .success-message {
      color: #4caf50;
      margin-bottom: 1rem;
      text-align: center;
    }
    
    .auth-links {
      display: flex;
      justify-content: center;
      width: 100%;
    }
    
    .auth-links a {
      text-decoration: none;
      color: var(--accent-color);
    }
  `]
})
export class SignupComponent {
  signupForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  errorDetails: any = null;
  loading$;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loading$ = this.authService.loading$;
    this.signupForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.pattern(/^[a-zA-Z\s]+$/)]],
      email: ['', [Validators.required, Validators.email, Validators.pattern(/^[^\s@]+@[^\s@]+\.[^\s@]+$/)]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^[+]?[0-9]{10,15}$/)]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$/)]]
    });
  }

  onSubmit(): void {
    if (this.signupForm.valid) {
      this.errorMessage = '';
      this.successMessage = '';
      
      this.authService.signUp(this.signupForm.value).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = 'Account created! Please check your email for verification code.';
            setTimeout(() => {
              this.router.navigate(['/auth/verify-otp'], { 
                queryParams: { email: this.signupForm.value.email } 
              });
            }, 2000);
          } else {
            this.errorMessage = response.message || 'Signup failed';
          }
        },
        error: (error) => {
          this.errorDetails = error;
          this.errorMessage = error?.message || 'Signup failed. Please try again.';
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.signupForm.controls).forEach(key => {
      const control = this.signupForm.get(key);
      control?.markAsTouched();
    });
  }
}