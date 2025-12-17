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
  selector: 'app-forgot-password',
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
          <mat-card-title>Reset Password</mat-card-title>
          <mat-card-subtitle>Enter your email to receive reset code</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="forgotForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" required>
              <mat-error *ngIf="forgotForm.get('email')?.hasError('required')">
                Email is required
              </mat-error>
              <mat-error *ngIf="forgotForm.get('email')?.hasError('email')">
                Please enter a valid email
              </mat-error>
            </mat-form-field>

            <div class="error-message" *ngIf="errorMessage">
              {{ errorMessage }}
            </div>

            <div class="success-message" *ngIf="successMessage">
              {{ successMessage }}
            </div>

            <button mat-raised-button color="primary" type="submit" 
                    [disabled]="forgotForm.invalid || (loading$ | async)" class="full-width">
              <mat-spinner diameter="20" *ngIf="loading$ | async"></mat-spinner>
              <span *ngIf="!(loading$ | async)">Send Reset Code</span>
            </button>
          </form>
        </mat-card-content>

        <mat-card-actions>
          <div class="auth-links">
            <a routerLink="/auth/login">Back to Login</a>
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
export class ForgotPasswordComponent {
  forgotForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  loading$;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loading$ = this.authService.loading$;
    this.forgotForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.forgotForm.valid) {
      this.errorMessage = '';
      this.successMessage = '';
      
      this.authService.forgotPassword(this.forgotForm.value).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = 'Reset code sent to your email!';
            setTimeout(() => {
              this.router.navigate(['/auth/reset-password'], { 
                queryParams: { email: this.forgotForm.value.email } 
              });
            }, 2000);
          }
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to send reset code';
        }
      });
    }
  }
}