import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-reset-password',
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
          <mat-card-title>Set New Password</mat-card-title>
          <mat-card-subtitle>Enter the code sent to your email and your new password</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="resetForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" readonly>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Reset Code</mat-label>
              <input matInput type="text" formControlName="code" required placeholder="Enter OTP">
              <mat-error *ngIf="resetForm.get('code')?.hasError('required')">
                Reset code is required
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>New Password</mat-label>
              <input matInput type="password" formControlName="newPassword" required>
              <mat-error *ngIf="resetForm.get('newPassword')?.hasError('required')">
                New password is required
              </mat-error>
              <mat-error *ngIf="resetForm.get('newPassword')?.hasError('minlength')">
                Password must be at least 6 characters
              </mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Confirm Password</mat-label>
              <input matInput type="password" formControlName="confirmPassword" required>
              <mat-error *ngIf="resetForm.get('confirmPassword')?.hasError('required')">
                Please confirm your password
              </mat-error>
              <mat-error *ngIf="resetForm.hasError('mismatch')">
                Passwords do not match
              </mat-error>
            </mat-form-field>

            <div class="error-message" *ngIf="errorMessage">
              {{ errorMessage }}
            </div>

            <div class="success-message" *ngIf="successMessage">
              {{ successMessage }}
            </div>

            <button mat-raised-button color="primary" type="submit" 
                    [disabled]="resetForm.invalid || (loading$ | async)" class="full-width">
              <mat-spinner diameter="20" *ngIf="loading$ | async"></mat-spinner>
              <span *ngIf="!(loading$ | async)">Reset Password</span>
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
export class ResetPasswordComponent implements OnInit {
  resetForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  loading$;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loading$ = this.authService.loading$;
    this.resetForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      code: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    const email = this.route.snapshot.queryParamMap.get('email');
    const code = this.route.snapshot.queryParamMap.get('code');

    if (email) {
      this.resetForm.patchValue({ email });
    }

    if (code) {
      this.resetForm.patchValue({ code });
    }

    if (!email && !code) {
      this.router.navigate(['/auth/forgot-password']);
    }
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  onSubmit(): void {
    if (this.resetForm.valid) {
      this.errorMessage = '';
      this.successMessage = '';

      const { email, code, newPassword } = this.resetForm.value;

      this.authService.resetPassword({ email, code, newPassword }).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = 'Password reset successfully! Redirecting to login...';
            setTimeout(() => {
              this.router.navigate(['/auth/login']);
            }, 2000);
          }
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to reset password';
        }
      });
    }
  }
}
