import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../core/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'accept-invitation',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule, MatIconModule],
  template: `
    <div class="wrap">
      <mat-card class="card">
        <h2 class="title">Accept Invitation</h2>

        <p class="subtitle">Click the invitation link in your email to open this page — the token is embedded automatically.</p>

        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <mat-form-field class="full-width">
            <mat-label>Set Password</mat-label>
            <input matInput [type]="showPassword ? 'text' : 'password'" formControlName="newPassword" />
            <button mat-icon-button matSuffix type="button" (click)="showPassword = !showPassword" aria-label="Toggle password visibility">
              <mat-icon>{{ showPassword ? 'visibility_off' : 'visibility' }}</mat-icon>
            </button>
          </mat-form-field>

          <div class="actions">
            <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid || loading || !token">Accept Invitation</button>
            <div class="status">
              <span class="success" *ngIf="success">{{ success }}</span>
              <span class="error" *ngIf="error">{{ error }}</span>
            </div>
          </div>
        </form>
      </mat-card>
    </div>
  `,
  styles: [`
    .wrap { display:flex; align-items:center; justify-content:center; padding:24px; }
    .card { width:100%; max-width:480px; padding:16px; box-sizing:border-box; }
    .title { margin:0 0 8px 0; }
    .subtitle { margin:0 0 16px 0; color:rgba(0,0,0,0.6); font-size:14px }
    .full-width { width:100%; margin-bottom:12px }
    .actions { display:flex; align-items:center; gap:12px }
    .status { display:flex; gap:8px; align-items:center }
    .success { color:green }
    .error { color:#c0392b }
  `]
})
export class AcceptInvitationComponent {
  form: FormGroup;
  loading = false;
  success = '';
  error = '';
  showPassword = false;
  token: string | null = null;

  constructor(private fb: FormBuilder, private auth: AuthService, private route: ActivatedRoute, private router: Router) {
    this.form = this.fb.group({ newPassword: ['', [Validators.required, Validators.minLength(8)]] });
    const token = this.route.snapshot.queryParamMap.get('token');
    if (token) {
      this.token = token;
    }
  }

  onSubmit() {
    if (this.form.invalid) return;
    if (!this.token) { this.error = 'Missing invitation token'; return; }
    this.loading = true;
    const newPassword = String(this.form.get('newPassword')?.value ?? '');
    this.auth.acceptInvite({ token: this.token, newPassword }).subscribe({
      next: (res) => {
        this.loading = false;
        this.success = res.message || 'Invitation accepted';
        this.form.reset();
        // Redirect to login so user can authenticate
        setTimeout(() => this.router.navigate(['/auth/login']), 1500);
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.message || 'Failed to accept invitation';
      }
    });
  }
  
}
