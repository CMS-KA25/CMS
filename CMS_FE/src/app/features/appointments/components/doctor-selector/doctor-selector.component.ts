import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { Doctor } from '../../../calendar/services/calendar.service';

@Component({
  selector: 'app-doctor-selector',
  standalone: true,
  imports: [CommonModule, MatFormFieldModule, MatSelectModule],
  templateUrl: './doctor-selector.component.html',
  styleUrl: './doctor-selector.component.css',
})
export class DoctorSelectorComponent {
  @Input() doctors: Doctor[] = [];
  @Input() selectedDrId = '';
  @Output() doctorChange = new EventEmitter<Doctor>();

  onDrSelect(id: string) {
    this.selectedDrId = id;
    const doc = this.doctors.find((d) => d.id === id);
    if (doc) this.doctorChange.emit(doc);
  }
}
