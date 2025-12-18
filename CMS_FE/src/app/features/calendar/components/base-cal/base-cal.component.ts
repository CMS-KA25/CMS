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
  @Input() userRole: number = 1; // Default to User role

  private _workingDays: number[] = [];
  @Input()
  set workingDays(value: number[]) {
    this._workingDays = value;
    this.dateFilter = this.createDateFilter();
  }
  get workingDays(): number[] {
    return this._workingDays;
  }

  get maxDate(): Date {
    const now = new Date();
    
    // Role-based booking limits:
    // Role 4 (Admin) → No limit (5 years ahead)
    // Role 2 (Staff) & Role 3 (Doctor) → 12 months
    // Role 1 (User), Role 5 (Patient) → 3 months
    let monthsAhead = 3; // Default for User/Patient
    
    if (this.userRole === 4) { // Admin - no limit
      monthsAhead = 60; // 5 years ahead
    } else if (this.userRole === 2 || this.userRole === 3) { // Staff or Doctor
      monthsAhead = 12;
    }
    
    return new Date(now.getFullYear(), now.getMonth() + monthsAhead, now.getDate());
  }

  dateFilter: (d: Date | null) => boolean = this.createDateFilter();

  private createDateFilter(): (d: Date | null) => boolean {
    return (d: Date | null) => {
      if (!d) return false;
      if (this.workingDays.length === 0) {
        return false; // No doctor selected, disable all dates
      }

      const jsDay = d.getDay(); // 0=Sunday, 1=Monday, ..., 6=Saturday
      // Convert JS day to WorkingDays enum: Sunday(0)->7, Monday(1)->1, ..., Saturday(6)->6
      const workingDay = jsDay === 0 ? 7 : jsDay;
      return this.workingDays.includes(workingDay);
    };
  }

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
