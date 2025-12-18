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
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { RouterOutlet, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { DoctorSelectorComponent } from '../appointments/components/doctor-selector/doctor-selector.component';
import { BaseCalComponent } from '../calendar/components/base-cal/base-cal.component';
import { TimeslotButtonComponent } from '../appointments/components/timeslot-button/timeslot-button.component';
import { CalendarService, Doctor } from '../calendar/services/calendar.service';

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
    MatSelectModule,
    MatDialogModule,
  ],
  templateUrl: './patient-dashboard.component.html',
  styleUrl: './patient-dashboard.component.css',
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
  specializations: string[] = [];
  selectedSpecialization: string = '';
  timeSlots: string[] = [];
  showProfileDialog = false;

  constructor(
    private authService: AuthService,
    private calendarService: CalendarService,
    private router: Router,
    private dialog: MatDialog
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
          this.specializations = [
            ...new Set(response.data.map((d) => d.specialization)),
          ];
        }
      },
      error: (error) => console.error('Error loading doctors:', error),
    });
  }

  onSpecializationChange(spec: string): void {
    this.selectedSpecialization = spec;
    if (spec) {
      this.filteredDoctors = this.doctors.filter(
        (d) => d.specialization === spec
      );
    } else {
      this.filteredDoctors = this.doctors;
    }

    // Clear selected doctor if they don't match the new specialization
    if (
      this.selectedDoctor &&
      this.selectedDoctor.specialization !== spec &&
      spec !== ''
    ) {
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
      this.calendarService
        .getAvailableSlots(this.selectedDoctor.id, formattedDate)
        .subscribe({
          next: (response) => {
            if (response.success) {
              this.timeSlots = response.data.availableSlots.map(
                (slot: any) => slot.displayTime
              );
            }
            this.loadingSlots = false;
          },
          error: (error) => {
            console.error('Error loading slots:', error);
            this.loadingSlots = false;
          },
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
    this.selectedDate = undefined; // Clear date when doctor changes
    this.selectedTimeSlot = undefined;
    this.timeSlots = []; // Clear slots since date is cleared
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
        notes: 'Booked via portal',
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
        error: (error) => alert('Error during booking.'),
      });
    }
  }

  isFormValid(): boolean {
    return !!(
      this.selectedDoctor &&
      this.selectedDate &&
      this.selectedTimeSlot
    );
  }

  openProfile(): void {
    this.showProfileDialog = true;
  }

  closeProfile(): void {
    this.showProfileDialog = false;
  }

  getRoleName(role: any): string {
    if (typeof role === 'number') {
      const roleMap: { [key: number]: string } = {
        1: 'Patient',
        2: 'Staff', 
        3: 'Doctor',
        4: 'Admin',
        5: 'User'
      };
      return roleMap[role] || 'Unknown';
    }
    return role || 'Unknown';
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}
