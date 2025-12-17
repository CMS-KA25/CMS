import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-verify-otp',
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
          <mat-card-title>Verify Email</mat-card-title>
          <mat-card-subtitle>Enter the verification code sent to {{ email }}</mat-card-subtitle>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="otpForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Verification Code</mat-label>
              <input matInput formControlName="code" required maxlength="6">
              <mat-error *ngIf="otpForm.get('code')?.hasError('required')">
                Verification code is required
              </mat-error>
            </mat-form-field>

            <div class="error-message" *ngIf="errorMessage">
              {{ errorMessage }}
            </div>

            <div class="success-message" *ngIf="successMessage">
              {{ successMessage }}
            </div>

            <button mat-raised-button color="primary" type="submit" 
                    [disabled]="otpForm.invalid || (loading$ | async)" class="full-width">
              <mat-spinner diameter="20" *ngIf="loading$ | async"></mat-spinner>
              <span *ngIf="!(loading$ | async)">Verify</span>
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
export class VerifyOtpComponent implements OnInit {
  otpForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  email = '';
  loading$;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loading$ = this.authService.loading$;
    this.otpForm = this.fb.group({
      code: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.email = params['email'] || '';
    });
  }

  onSubmit(): void {
    if (this.otpForm.valid && this.email) {
      this.errorMessage = '';
      this.successMessage = '';
      
      const otpData = {
        email: this.email,
        code: this.otpForm.value.code,
        purpose: 'signup'
      };

      this.authService.verifyOtp(otpData).subscribe({
        next: (response) => {
          if (response.success) {
            this.successMessage = 'Email verified successfully!';
            setTimeout(() => {
              this.router.navigate(['/auth/login']);
            }, 2000);
          }
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Invalid verification code';
        }
      });
    }
  }
}