import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { RouterOutlet, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

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
    RouterOutlet
  ],
  template: `
    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav #drawer class="sidenav" fixedInViewport mode="side" opened>
        <mat-toolbar>Patient Portal</mat-toolbar>
        <mat-nav-list>
          <a mat-list-item (click)="setActiveSection('book-appointment')">
            <mat-icon>event</mat-icon>
            <span>Book Appointment</span>
          </a>
          <a mat-list-item (click)="setActiveSection('upcoming')">
            <mat-icon>schedule</mat-icon>
            <span>Upcoming Appointments</span>
          </a>
          <a mat-list-item (click)="setActiveSection('checkin')">
            <mat-icon>check_circle</mat-icon>
            <span>Check-in</span>
          </a>
          <a mat-list-item (click)="setActiveSection('health-records')">
            <mat-icon>folder_shared</mat-icon>
            <span>Electronic Health Records</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <mat-toolbar color="primary">
          <span>Patient Dashboard</span>
          <span class="spacer"></span>
          <span>Welcome, {{ currentUser?.name }}</span>
          <button mat-icon-button (click)="logout()">
            <mat-icon>logout</mat-icon>
          </button>
        </mat-toolbar>

        <div class="content">
          <div [ngSwitch]="activeSection">
            <div *ngSwitchCase="'book-appointment'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Book New Appointment</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Schedule your next appointment with available doctors.</p>
                  <!-- Appointment booking form will go here -->
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchCase="'upcoming'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Upcoming Appointments</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>View your scheduled appointments.</p>
                  <!-- Upcoming appointments list will go here -->
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchCase="'checkin'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Check-in</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Check-in for your appointment.</p>
                  <!-- Check-in form will go here -->
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchCase="'health-records'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Electronic Health Records</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Access your medical records and test results.</p>
                  <!-- Health records will go here -->
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchDefault>
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Welcome to Patient Portal</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Select an option from the sidebar to get started.</p>
                </mat-card-content>
              </mat-card>
            </div>
          </div>
        </div>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .sidenav-container {
      height: 100vh;
    }
    
    .sidenav {
      width: 250px;
    }
    
    .spacer {
      flex: 1 1 auto;
    }
    
    .content {
      padding: 20px;
    }
    
    mat-card {
      margin-bottom: 20px;
    }
  `]
})
export class PatientDashboardComponent {
  activeSection = 'book-appointment';
  currentUser: any;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    this.currentUser = this.authService.getCurrentUser();
  }

  setActiveSection(section: string): void {
    this.activeSection = section;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}