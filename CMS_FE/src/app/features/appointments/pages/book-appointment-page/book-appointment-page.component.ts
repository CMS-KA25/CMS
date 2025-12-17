import { Component } from '@angular/core';
import { DoctorSelectorComponent } from '../../components/doctor-selector/doctor-selector.component';
import { BaseCalComponent } from '../../../calendar/components/base-cal/base-cal.component';
import { TimeslotButtonComponent } from '../../components/timeslot-button/timeslot-button.component';
import { MatButtonModule } from '@angular/material/button';

interface DoctorOption {
  id: string;
  name: string;
  specialization: string;
}

@Component({
  selector: 'app-book-appointment-page',
  standalone: true,
  imports: [DoctorSelectorComponent, BaseCalComponent, TimeslotButtonComponent, MatButtonModule],
  templateUrl: './book-appointment-page.component.html',
  styleUrl: './book-appointment-page.component.css'
})
export class BookAppointmentPageComponent {
  selectedDoctor?: DoctorOption;
  selectedDate?: Date;
  selectedTimeSlot?: string;

  doctors: DoctorOption[] = [
    { id: '1', name: 'Dr. Smith', specialization: 'Cardiology' },
    { id: '2', name: 'Dr. Johnson', specialization: 'Dermatology' },
    { id: '3', name: 'Dr. Williams', specialization: 'Neurology' }
  ];

  timeSlots: string[] = [
    '9:00 AM', '9:30 AM', '10:00 AM', '10:30 AM', '11:00 AM', '11:30 AM',
    '2:00 PM', '2:30 PM', '3:00 PM', '3:30 PM', '4:00 PM', '4:30 PM'
  ];

  onDoctorChange(doctor: DoctorOption) {
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
