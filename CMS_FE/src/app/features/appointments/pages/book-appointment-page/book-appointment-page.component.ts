import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DoctorSelectorComponent } from '../../components/doctor-selector/doctor-selector.component';
import { BaseCalComponent } from '../../../calendar/components/base-cal/base-cal.component';
import { TimeslotButtonComponent } from '../../components/timeslot-button/timeslot-button.component';
import { MatButtonModule } from '@angular/material/button';
import { Doctor } from '../../../calendar/services/calendar.service';

@Component({
  selector: 'app-book-appointment-page',
  standalone: true,
  imports: [CommonModule, DoctorSelectorComponent, BaseCalComponent, TimeslotButtonComponent, MatButtonModule],
  templateUrl: './book-appointment-page.component.html',
  styleUrl: './book-appointment-page.component.css'
})
export class BookAppointmentPageComponent {
  selectedDoctor?: Doctor;
  selectedDate?: Date;
  selectedTimeSlot?: string;

  doctors: Doctor[] = []; // This should ideally be loaded from a service, but keeping as is for now

  timeSlots: string[] = [
    '9:00 AM', '9:30 AM', '10:00 AM', '10:30 AM', '11:00 AM', '11:30 AM',
    '2:00 PM', '2:30 PM', '3:00 PM', '3:30 PM', '4:00 PM', '4:30 PM'
  ];

  onDoctorChange(doctor: Doctor) {
    this.selectedDoctor = doctor;
  }

  onDateChange(date: Date) {
    this.selectedDate = date;
  }

  onTimeSlotChange(slot: string) {
    this.selectedTimeSlot = slot;
  }

  onProceed() {
    const appointmentData = {
      doctor: this.selectedDoctor,
      date: this.selectedDate,
      timeSlot: this.selectedTimeSlot
    };
    console.log('Appointment Data:', appointmentData);
  }

  isFormValid(): boolean {
    return !!(this.selectedDoctor && this.selectedDate && this.selectedTimeSlot);
  }
}
