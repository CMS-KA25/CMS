import { Component, EventEmitter, Input, Output, signal } from '@angular/core';

import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-base-cal',
  standalone: true,
  imports: [MatDatepickerModule, MatNativeDateModule],
  templateUrl: './base-cal.component.html',
  styleUrl: './base-cal.component.css',
})
export class BaseCalComponent {
  selectedDate = signal<Date>(new Date());
  @Input() minDate: Date = new Date();
  @Input() startAt: Date = new Date();
  @Input() startView: 'month' | 'year' | 'multi-year' = 'month';

  @Input() maxDate: Date = (() => {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth() + 3, now.getDate());
  })();
  @Input() dateFilter: (d: Date | null) => boolean = (d: Date | null) => {
    const date = d ?? new Date();
    const day = date.getDay();
    return day !== 0 && day !== 6;
  };

  @Input() dateClass: (d: Date) => string = (d: Date) => {
    const day = d.getDay();
    return day === 0 || day === 6 ? 'weekend-date' : '';
  };

  @Output() selectedChange = new EventEmitter<Date>();

  onSelectedChange(date: Date | null) {
    if (date) {
      this.selectedDate.set(date);
      this.selectedChange.emit(date);
    }
  }
}
