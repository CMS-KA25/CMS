import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BookAppointmentPageComponent } from './features/appointments/pages/book-appointment-page/book-appointment-page.component';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, BookAppointmentPageComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  title = 'CMS_FE';

  constructor(private themeService: ThemeService) {}

  ngOnInit(): void {
    // Theme service automatically initializes system theme detection
  }
}
