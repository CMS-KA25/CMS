import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { DoctorService } from '../../services/doctor.service';
import { SPECIALIZATIONS } from '../../../../shared/constants/specializations';

@Component({
  selector: 'app-doctor-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatSnackBarModule
  ],
  template: `
    <mat-card class="profile-card">
      <mat-card-header>
        <mat-card-title>Doctor Profile Setup</mat-card-title>
        <mat-card-subtitle>Complete your professional details to start receiving appointments</mat-card-subtitle>
      </mat-card-header>
      
      <mat-card-content>
        <form [formGroup]="profileForm" (ngSubmit)="onSubmit()" class="profile-form">
          <div class="form-row">
            <mat-form-field appearance="outline">
              <mat-label>Specialization</mat-label>
              <mat-select formControlName="specialization">
                <mat-option *ngFor="let spec of specializations" [value]="spec">{{spec}}</mat-option>
              </mat-select>
              <mat-error *ngIf="profileForm.get('specialization')?.invalid">Specialization is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Qualification</mat-label>
              <input matInput formControlName="qualification" placeholder="e.g. MBBS, MD">
              <mat-error *ngIf="profileForm.get('qualification')?.invalid">Qualification is required</mat-error>
            </mat-form-field>
          </div>

          <div class="form-row">
            <mat-form-field appearance="outline">
              <mat-label>Years of Experience</mat-label>
              <input matInput type="number" formControlName="yearOfExperience">
              <mat-error *ngIf="profileForm.get('yearOfExperience')?.invalid">Valid experience is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Slot Duration (minutes)</mat-label>
              <mat-select formControlName="slotDuration">
                <mat-option [value]="15">15 mins</mat-option>
                <mat-option [value]="30">30 mins</mat-option>
                <mat-option [value]="45">45 mins</mat-option>
                <mat-option [value]="60">60 mins</mat-option>
              </mat-select>
            </mat-form-field>
          </div>

          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Working Days</mat-label>
            <mat-select formControlName="workingDays" multiple>
              <mat-option *ngFor="let day of days" [value]="day">{{day}}</mat-option>
            </mat-select>
          </mat-form-field>

          <div class="form-row">
            <mat-form-field appearance="outline">
              <mat-label>Start Time</mat-label>
              <input matInput type="time" formControlName="startTime">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>End Time</mat-label>
              <input matInput type="time" formControlName="endTime">
            </mat-form-field>
          </div>

          <div class="form-row">
            <mat-form-field appearance="outline">
              <mat-label>Break Start Time (Optional)</mat-label>
              <input matInput type="time" formControlName="breakStartTime">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Break End Time (Optional)</mat-label>
              <input matInput type="time" formControlName="breakEndTime">
            </mat-form-field>
          </div>

          <div class="actions">
            <button mat-raised-button color="primary" type="submit" [disabled]="profileForm.invalid || loading">
              {{ loading ? 'Saving...' : 'Save Profile' }}
            </button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .profile-card {
      max-width: 800px;
      margin: 20px auto;
      padding: 20px;
    }
    .profile-form {
      display: flex;
      flex-direction: column;
      gap: 15px;
      margin-top: 20px;
    }
    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 20px;
    }
    .full-width {
      width: 100%;
    }
    .actions {
      display: flex;
      justify-content: flex-end;
      margin-top: 20px;
    }
    @media (max-width: 600px) {
      .form-row {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class DoctorProfileComponent implements OnInit {
  profileForm: FormGroup;
  loading = false;
  days = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  specializations = SPECIALIZATIONS;

  constructor(
    private fb: FormBuilder,
    private doctorService: DoctorService,
    private snackBar: MatSnackBar
  ) {
    this.profileForm = this.fb.group({
      specialization: ['', Validators.required],
      qualification: ['', Validators.required],
      yearOfExperience: [0, [Validators.required, Validators.min(0)]],
      workingDays: [[], Validators.required],
      startTime: ['09:00', Validators.required],
      endTime: ['17:00', Validators.required],
      slotDuration: [30, Validators.required],
      breakStartTime: [''],
      breakEndTime: ['']
    });
  }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading = true;
    this.doctorService.getProfile().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          const data = response.data;
          this.profileForm.patchValue({
            specialization: data.specialization,
            qualification: data.qualification,
            yearOfExperience: data.yearOfExperience,
            workingDays: data.workingDays || [],
            startTime: this.formatTime(data.startTime),
            endTime: this.formatTime(data.endTime),
            slotDuration: data.slotDuration,
            breakStartTime: this.formatTime(data.breakStartTime),
            breakEndTime: this.formatTime(data.breakEndTime)
          });
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  private formatTime(time: any): string {
    if (!time) return '';
    // If it's a TimeSpan string from .NET (e.g. "09:00:00")
    if (typeof time === 'string') {
      return time.substring(0, 5);
    }
    return '';
  }

  onSubmit(): void {
    if (this.profileForm.valid) {
      this.loading = true;
      const val = this.profileForm.value;

      // Ensure times are in "HH:mm:ss" format for .NET TimeSpan
      const payload = {
        ...val,
        startTime: val.startTime + ':00',
        endTime: val.endTime + ':00',
        breakStartTime: val.breakStartTime ? val.breakStartTime + ':00' : null,
        breakEndTime: val.breakEndTime ? val.breakEndTime + ':00' : null
      };

      this.doctorService.updateProfile(payload).subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success) {
            this.snackBar.open('Profile updated successfully!', 'Close', { duration: 3000 });
          }
        },
        error: (err) => {
          this.loading = false;
          this.snackBar.open(err.error?.message || 'Failed to update profile', 'Close', { duration: 3000 });
        }
      });
    }
  }
}
