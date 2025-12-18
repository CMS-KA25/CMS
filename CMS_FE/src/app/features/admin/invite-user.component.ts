import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { AuthService } from '../../core/services/auth.service';
import { RoleType } from '../../shared/models/auth.models';

@Component({
  selector: 'invite-user',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule, MatCardModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="onSubmit()">
      <mat-form-field class="full-width">
        <mat-label>Email</mat-label>
        <input matInput formControlName="email" required />
      </mat-form-field>

      <mat-form-field class="full-width">
        <mat-label>Name</mat-label>
        <input matInput formControlName="name" />
      </mat-form-field>

      <mat-form-field class="full-width">
        <mat-label>Phone</mat-label>
        <input matInput formControlName="phoneNumber" />
      </mat-form-field>

      <mat-form-field class="full-width">
        <mat-label>Role</mat-label>
        <mat-select formControlName="role">
          <mat-option [value]="RoleType.Staff">Staff</mat-option>
          <mat-option [value]="RoleType.Doctor">Doctor</mat-option>
        </mat-select>
      </mat-form-field>

      <div style="display:flex;gap:8px;align-items:center">
        <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid || loading">Invite</button>
        <span *ngIf="success" style="color:green">{{ success }}</span>
        <span *ngIf="error" style="color:red">{{ error }}</span>
      </div>
    </form>
  `,
  styles: ['.full-width { width: 100%; margin-bottom: 8px; }']
})
export class InviteUserComponent {
  RoleType = RoleType;
  form: any;
  loading = false;
  success = '';
  error = '';

  constructor(private fb: FormBuilder, private auth: AuthService) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      name: ['', [Validators.required, Validators.minLength(2)]],
      phoneNumber: ['', [Validators.required, Validators.pattern(/^[+]?[0-9]{7,20}$/)]],
      role: [RoleType.Staff, Validators.required]
    });
  }

  onSubmit() {
    if (this.form.invalid) return;
    this.loading = true;
    this.success = '';
    this.error = '';
    const payload = {
      email: String(this.form.get('email')?.value ?? ''),
      name: String(this.form.get('name')?.value ?? ''),
      phoneNumber: String(this.form.get('phoneNumber')?.value ?? ''),
      role: this.form.get('role')?.value
    };

    this.auth.inviteUser(payload).subscribe({
      next: (res) => {
        this.loading = false;
        this.success = res.message || 'Invitation sent';
        this.form.reset({ role: RoleType.Staff });
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.message || 'Failed to invite';
      }
    });
  }
}
