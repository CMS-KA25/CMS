import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';

import { CalendarService, Doctor, TimeSlot, Appointment } from '../../services/calendar.service';
import { AuthService } from '../../../../core/services/auth.service';
import { RoleType } from '../../../../shared/models/auth.models';

@Component({
    selector: 'app-doctor-calendar',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatCardModule,
        MatButtonModule,
        MatSelectModule,
        MatFormFieldModule,
        MatProgressSpinnerModule,
        MatSnackBarModule,
        MatChipsModule,
        MatIconModule
    ],
    templateUrl: './doctor-calendar.component.html',
    styleUrl: './doctor-calendar.component.css'
})
export class DoctorCalendarComponent implements OnInit {
    private calendarService = inject(CalendarService);
    private authService = inject(AuthService);
    private snackBar = inject(MatSnackBar);

    selectedDate = signal<Date>(new Date());
    doctors = signal<Doctor[]>([]);
    selectedDoctor = signal<Doctor | null>(null);
    availableSlots = signal<TimeSlot[]>([]);
    appointments = signal<Appointment[]>([]);
    loading = signal<boolean>(false);

    currentUser = this.authService.getCurrentUser();
    isDoctor = this.currentUser?.role === RoleType.Doctor;
    isPatient = this.currentUser?.role === RoleType.User;

    minDate: Date = new Date();
    maxDate: Date = (() => {
        const now = new Date();
        return new Date(now.getFullYear(), now.getMonth() + 3, now.getDate());
    })();

    constructor() { }

    ngOnInit() {
        this.loadDoctors();

        // If current user is a doctor, auto-select them
        if (this.isDoctor && this.currentUser) {
            this.loadDoctorAppointments();
        }
    }

    loadDoctors() {
        this.loading.set(true);
        this.calendarService.getAllDoctors().subscribe({
            next: (response: any) => {
                if (response.success) {
                    this.doctors.set(response.data);

                    // Auto-select current doctor if user is a doctor
                    if (this.isDoctor && this.currentUser) {
                        const currentDoctor = response.data.find((d: any) => d.id === this.currentUser!.id);
                        if (currentDoctor) {
                            this.selectedDoctor.set(currentDoctor);
                        }
                    }
                }
                this.loading.set(false);
            },
            error: (error: any) => {
                this.showError('Failed to load doctors');
                this.loading.set(false);
            }
        });
    }

    onDoctorSelected(doctor: Doctor) {
        this.selectedDoctor.set(doctor);
        this.loadAvailableSlots();
    }

    onDateSelected(date: Date) {
        this.selectedDate.set(date);
        if (this.selectedDoctor()) {
            this.loadAvailableSlots();
        }
    }

    loadAvailableSlots() {
        const doctor = this.selectedDoctor();
        if (!doctor) return;

        this.loading.set(true);
        const date = this.selectedDate();
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const dateStr = `${year}-${month}-${day}`;

        this.calendarService.getAvailableSlots(doctor.id, dateStr).subscribe({
            next: (response: any) => {
                if (response.success) {
                    this.availableSlots.set(response.data);
                }
                this.loading.set(false);
            },
            error: (error: any) => {
                this.showError('Failed to load available slots');
                this.loading.set(false);
            }
        });
    }

    loadDoctorAppointments() {
        if (!this.currentUser) return;

        this.loading.set(true);
        const startDate = new Date();
        const endDate = new Date();
        endDate.setMonth(endDate.getMonth() + 1);

        this.calendarService.getDoctorAppointments(
            this.currentUser.id,
            startDate.toISOString().split('T')[0],
            endDate.toISOString().split('T')[0]
        ).subscribe({
            next: (response: any) => {
                if (response.success) {
                    this.appointments.set(response.data);
                }
                this.loading.set(false);
            },
            error: (error: any) => {
                this.showError('Failed to load appointments');
                this.loading.set(false);
            }
        });
    }

    bookSlot(slot: TimeSlot) {
        if (!this.currentUser || !this.selectedDoctor()) {
            this.showError('Please select a doctor and ensure you are logged in');
            return;
        }

        const appointment: Appointment = {
            doctorId: this.selectedDoctor()!.id,
            patientId: this.currentUser.id,
            appointmentDate: this.selectedDate().toISOString().split('T')[0],
            startTime: slot.startTime,
            endTime: slot.endTime,
            status: 'Scheduled'
        };

        this.loading.set(true);
        this.calendarService.bookAppointment(appointment).subscribe({
            next: (response: any) => {
                if (response.success) {
                    this.showSuccess('Appointment booked successfully!');
                    this.loadAvailableSlots();
                }
                this.loading.set(false);
            },
            error: (error: any) => {
                this.showError('Failed to book appointment');
                this.loading.set(false);
            }
        });
    }

    cancelAppointment(appointmentId: string) {
        if (!confirm('Are you sure you want to cancel this appointment?')) {
            return;
        }

        this.loading.set(true);
        this.calendarService.cancelAppointment(appointmentId).subscribe({
            next: (response: any) => {
                if (response.success) {
                    this.showSuccess('Appointment cancelled successfully');
                    this.loadDoctorAppointments();
                }
                this.loading.set(false);
            },
            error: (error: any) => {
                this.showError('Failed to cancel appointment');
                this.loading.set(false);
            }
        });
    }

    private showSuccess(message: string) {
        this.snackBar.open(message, 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
        });
    }

    private showError(message: string) {
        this.snackBar.open(message, 'Close', {
            duration: 5000,
            panelClass: ['error-snackbar']
        });
    }

    // Helper method to check if a date should be disabled
    dateFilter = (d: Date | null): boolean => {
        const date = d ?? new Date();
        const day = date.getDay();
        // Disable weekends by default (can be customized based on doctor's working days)
        return day !== 0 && day !== 6;
    };
}
