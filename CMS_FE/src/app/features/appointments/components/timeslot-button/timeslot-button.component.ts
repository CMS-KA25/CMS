import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-timeslot-button',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  templateUrl: './timeslot-button.component.html',
  styleUrl: './timeslot-button.component.css',
})
export class TimeslotButtonComponent {
  @Input() slots: string[] = [];
  @Input() selectedSlot?: string;
  @Output() slotChange = new EventEmitter<string>();

  select(slot: string) {
    this.slotChange.emit(slot);
  }
}
