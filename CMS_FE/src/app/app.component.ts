import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BookAppointmentPageComponent } from './features/appointments/pages/book-appointment-page/book-appointment-page.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, BookAppointmentPageComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'CMS_FE';
}
