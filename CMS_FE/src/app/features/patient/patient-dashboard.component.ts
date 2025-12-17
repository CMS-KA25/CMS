import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { RouterOutlet, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { DoctorSelectorComponent } from '../appointments/components/doctor-selector/doctor-selector.component';
import { BaseCalComponent } from '../calendar/components/base-cal/base-cal.component';
import { TimeslotButtonComponent } from '../appointments/components/timeslot-button/timeslot-button.component';
import { CalendarService, Doctor } from '../calendar/services/calendar.service';
import { SPECIALIZATIONS } from '../../shared/constants/specializations';

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatCardModule,
    MatDividerModule,
    RouterOutlet,
    DoctorSelectorComponent,
    BaseCalComponent,
    TimeslotButtonComponent,
    MatFormFieldModule,
    MatSelectModule
  ],
  template: `
    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav #drawer class="sidenav" fixedInViewport mode="side" opened>
        <div class="sidebar-header">
          <mat-icon color="primary">medical_services</mat-icon>
          <span>Patient Portal</span>
        </div>
        
        <mat-nav-list>
          <a mat-list-item (click)="setActiveSection('book-appointment')" [class.active]="activeSection === 'book-appointment'">
            <mat-icon matListItemIcon>event</mat-icon>
            <span matListItemTitle>Book Appointment</span>
          </a>
          <a mat-list-item (click)="setActiveSection('upcoming')" [class.active]="activeSection === 'upcoming'">
            <mat-icon matListItemIcon>schedule</mat-icon>
            <span matListItemTitle>My Appointments</span>
          </a>
          <a mat-list-item (click)="setActiveSection('check-in')" [class.active]="activeSection === 'check-in'">
            <mat-icon matListItemIcon>login</mat-icon>
            <span matListItemTitle>Check-in</span>
          </a>
          <a mat-list-item (click)="setActiveSection('health-records')" [class.active]="activeSection === 'health-records'">
            <mat-icon matListItemIcon>folder_shared</mat-icon>
            <span matListItemTitle>Health Records</span>
          </a>
        </mat-nav-list>

        <div class="sidebar-footer">
          <mat-divider></mat-divider>
          <button mat-button (click)="logout()" class="logout-btn">
            <mat-icon>logout</mat-icon>
            <span>Logout</span>
          </button>
        </div>
      </mat-sidenav>

      <mat-sidenav-content>
        <div class="main-content">
          <div [ngSwitch]="activeSection">
            
            <!-- Book Appointment Section -->
            <div *ngSwitchCase="'book-appointment'">
              <h1 class="page-title">Book an Appointment</h1>
              
              <div class="booking-layout">
                <!-- Selection Card -->
                <mat-card class="booking-card">
                  <div class="form-group">
                    <label>1. Select Specialization</label>
                    <mat-form-field appearance="outline" class="full-width-field">
                      <mat-label>Choose category</mat-label>
                      <mat-select 
                        [(value)]="selectedSpecialization" 
                        (selectionChange)="onSpecializationChange($event.value)"
                      >
                        <mat-option [value]="''">All Specialties</mat-option>
                        <mat-option *ngFor="let spec of specializations" [value]="spec">
                          {{ spec }}
                        </mat-option>
                      </mat-select>
                    </mat-form-field>
                  </div>

                  <div class="form-group">
                    <label>2. Select Specialist</label>
                    <app-doctor-selector
                      [doctors]="filteredDoctors"
                      [selectedDrId]="selectedDoctor?.id || ''"
                      (doctorChange)="onDoctorChange($event)"
                    >
                    </app-doctor-selector>
                  </div>

                  <div class="form-group">
                    <label>3. Select Date</label>
                    <app-base-cal (selectedChange)="onDateChange($event)"></app-base-cal>
                  </div>
                </mat-card>

                <!-- Slots Card -->
                <mat-card class="booking-card slots-card">
                  <label>4. Available Time Slots</label>
                  
                  <div class="slots-container">
                    <div *ngIf="loadingSlots" class="loading-overlay">
                      <mat-icon class="spin">sync</mat-icon>
                      <span>Finding slots...</span>
                    </div>

                    <div *ngIf="!loadingSlots && timeSlots.length === 0" class="empty-slots">
                      <mat-icon>info</mat-icon>
                      <p>Select a doctor and date to check availability.</p>
                    </div>

                    <app-timeslot-button
                      *ngIf="!loadingSlots && timeSlots.length > 0"
                      [slots]="timeSlots"
                      [selectedSlot]="selectedTimeSlot"
                      (slotChange)="onTimeSlotChange($event)"
                    >
                    </app-timeslot-button>
                  </div>

                  <div class="booking-action" *ngIf="isFormValid()">
                    <button mat-raised-button color="primary" class="confirm-btn" (click)="onProceed()">
                      Confirm Selection
                    </button>
                    <p class="summary">
                      {{ selectedDoctor?.name }} on {{ selectedDate | date:'fullDate' }}
                    </p>
                  </div>
                </mat-card>
              </div>
            </div>

            <!-- Upcoming Appointments -->
            <div *ngSwitchCase="'upcoming'">
              <h1 class="page-title">My Appointments</h1>
              <mat-card class="info-card">
                <mat-card-content>
                  <p>No upcoming appointments found. Start by booking one!</p>
                  <button mat-stroked-button color="primary" (click)="setActiveSection('book-appointment')">
                    Book Now
                  </button>
                </mat-card-content>
              </mat-card>
            </div>

            <!-- Check-in -->
            <div *ngSwitchCase="'check-in'">
              <h1 class="page-title">Check-in</h1>
              <mat-card class="info-card">
                <mat-card-content>
                  <p>Check-in for your today's appointments.</p>
                </mat-card-content>
              </mat-card>
            </div>

            <!-- Health Records -->
            <div *ngSwitchCase="'health-records'">
              <h1 class="page-title">Health Records</h1>
              <mat-card class="info-card">
                <mat-card-content>
                  <p>Access your medical history and lab results here.</p>
                </mat-card-content>
              </mat-card>
            </div>

            <!-- Welcome Screen -->
            <div *ngSwitchDefault>
              <div class="welcome-section">
                <h1>Hello, {{ currentUser?.name }}</h1>
                <p>Welcome to your personal health portal. How can we help you today?</p>
                
                <div class="quick-actions">
                  <mat-card class="action-card" (click)="setActiveSection('book-appointment')">
                    <mat-icon color="primary">event</mat-icon>
                    <h3>Book Appointment</h3>
                    <p>Schedule a visit with a specialist.</p>
                  </mat-card>
                </div>
              </div>
            </div>

          </div>
        </div>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .sidenav-container {
      height: 100vh;
      background-color: #f4f7f9;
    }
    .sidenav {
      width: 260px;
      border-right: none;
      background: white;
      box-shadow: 2px 0 10px rgba(0,0,0,0.03);
    }
    .sidebar-header {
      padding: 40px 24px;
      display: flex;
      align-items: center;
      gap: 12px;
      font-size: 20px;
      font-weight: 700;
      color: #1e293b;
    }
    .sidebar-footer {
      position: absolute;
      bottom: 0;
      width: 100%;
      padding: 16px;
    }
    .logout-btn {
      width: 100%;
      text-align: left;
      color: #ef4444;
      display: flex;
      align-items: center;
      gap: 8px;
    }
    .main-content {
      padding: 48px;
      max-width: 1100px;
      margin: 0 auto;
    }
    .page-title {
      font-size: 32px;
      font-weight: 800;
      color: #0f172a;
      margin-bottom: 32px;
      letter-spacing: -0.5px;
    }
    .booking-layout {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 32px;
      align-items: start;
    }
    .booking-card {
      padding: 32px;
      border-radius: 20px;
      box-shadow: 0 10px 30px rgba(0,0,0,0.04) !important;
      border: 1px solid rgba(0,0,0,0.02);
    }
    .form-group {
      margin-bottom: 32px;
    }
    .form-group label, .slots-card label {
      display: block;
      font-size: 14px;
      font-weight: 600;
      color: #64748b;
      margin-bottom: 16px;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }
    .full-width-field {
      width: 100%;
    }
    .slots-container {
      min-height: 200px;
      display: flex;
      flex-direction: column;
    }
    .loading-overlay, .empty-slots {
      flex: 1;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      color: #94a3b8;
      text-align: center;
      gap: 12px;
    }
    .spin {
      animation: rotate 2s linear infinite;
    }
    @keyframes rotate {
      from { transform: rotate(0deg); }
      to { transform: rotate(360deg); }
    }
    .booking-action {
      margin-top: 32px;
      padding-top: 24px;
      border-top: 1px solid #f1f5f9;
      text-align: center;
    }
    .confirm-btn {
      width: 100%;
      height: 54px;
      border-radius: 12px;
      font-size: 16px;
      font-weight: 600;
    }
    .summary {
      margin-top: 12px;
      color: #64748b;
      font-size: 14px;
    }
    .welcome-section {
      text-align: center;
      padding-top: 60px;
    }
    .welcome-section h1 {
      font-size: 48px;
      font-weight: 800;
      color: #0f172a;
    }
    .quick-actions {
      margin-top: 48px;
      display: flex;
      justify-content: center;
    }
    .action-card {
      width: 300px;
      padding: 32px;
      cursor: pointer;
      transition: all 0.3s ease;
      text-align: center;
    }
    .action-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 20px 40px rgba(0,0,0,0.08) !important;
    }
    .action-card mat-icon {
      font-size: 40px;
      width: 40px;
      height: 40px;
      margin-bottom: 16px;
    }
    .active {
      color: #1976d2 !important;
      background: #f0f7ff;
    }
  `]
})
export class PatientDashboardComponent implements OnInit {
  activeSection = 'book-appointment';
  currentUser: any;

  selectedDoctor?: Doctor;
  selectedDate?: Date;
  selectedTimeSlot?: string;
  loadingSlots = false;

  doctors: Doctor[] = [];
  filteredDoctors: Doctor[] = [];
  specializations: string[] = SPECIALIZATIONS;
  selectedSpecialization: string = '';
  timeSlots: string[] = [];

  constructor(
    private authService: AuthService,
    private calendarService: CalendarService,
    private router: Router
  ) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit(): void {
    this.loadDoctors();
  }

  loadDoctors(): void {
    this.calendarService.getAllDoctors().subscribe({
      next: (response) => {
        if (response.success) {
          this.doctors = response.data;
          this.filteredDoctors = response.data;
        }
      },
      error: (error) => console.error('Error loading doctors:', error)
    });
  }

  onSpecializationChange(spec: string): void {
    this.selectedSpecialization = spec;
    if (spec) {
      this.filteredDoctors = this.doctors.filter(d => d.specialization === spec);
    } else {
      this.filteredDoctors = this.doctors;
    }

    // Clear selected doctor if they don't match the new specialization
    if (this.selectedDoctor && this.selectedDoctor.specialization !== spec && spec !== '') {
      this.selectedDoctor = undefined;
      this.selectedTimeSlot = undefined;
      this.timeSlots = [];
    }
  }

  loadAvailableSlots(): void {
    if (this.selectedDoctor && this.selectedDate) {
      this.loadingSlots = true;
      const year = this.selectedDate.getFullYear();
      const month = String(this.selectedDate.getMonth() + 1).padStart(2, '0');
      const day = String(this.selectedDate.getDate()).padStart(2, '0');
      const formattedDate = `${year}-${month}-${day}`;
      this.calendarService.getAvailableSlots(this.selectedDoctor.id, formattedDate).subscribe({
        next: (response) => {
          if (response.success) {
            this.timeSlots = response.data.availableSlots.map((slot: any) => slot.displayTime);
          }
          this.loadingSlots = false;
        },
        error: (error) => {
          console.error('Error loading slots:', error);
          this.loadingSlots = false;
        }
      });
    }
  }

  setActiveSection(section: string): void {
    this.activeSection = section;
    if (section !== 'book-appointment') {
      this.selectedDoctor = undefined;
      this.selectedDate = undefined;
      this.selectedTimeSlot = undefined;
      this.timeSlots = [];
    }
  }

  onDoctorChange(doctor: Doctor) {
    this.selectedDoctor = doctor;
    this.selectedTimeSlot = undefined;
    this.loadAvailableSlots();
  }

  onDateChange(date: Date) {
    this.selectedDate = date;
    this.selectedTimeSlot = undefined;
    this.loadAvailableSlots();
  }

  onTimeSlotChange(slot: string) {
    this.selectedTimeSlot = slot;
  }

  onProceed() {
    if (this.isFormValid()) {
      const formattedDate = this.selectedDate!.toISOString().split('T')[0];
      const appointment = {
        doctorId: this.selectedDoctor!.id,
        patientId: this.currentUser.id,
        appointmentDate: formattedDate,
        startTime: this.selectedTimeSlot!,
        endTime: '',
        status: 'Scheduled',
        notes: 'Booked via portal'
      };

      this.calendarService.bookAppointment(appointment as any).subscribe({
        next: (response) => {
          if (response.success) {
            alert(`Successfully booked!`);
            this.setActiveSection('upcoming');
          } else {
            alert('Failed: ' + response.message);
          }
        },
        error: (error) => alert('Error during booking.')
      });
    }
  }

  isFormValid(): boolean {
    return !!(this.selectedDoctor && this.selectedDate && this.selectedTimeSlot);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}