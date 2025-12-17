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
  selector: 'app-doctor-dashboard',
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
        <mat-toolbar>Doctor Portal</mat-toolbar>
        <mat-nav-list>
          <a mat-list-item (click)="setActiveSection('appointments')">
            <mat-icon>event</mat-icon>
            <span>My Appointments</span>
          </a>
          <a mat-list-item (click)="setActiveSection('patients')">
            <mat-icon>people</mat-icon>
            <span>Patient Records</span>
          </a>
          <a mat-list-item (click)="setActiveSection('schedule')">
            <mat-icon>schedule</mat-icon>
            <span>My Schedule</span>
          </a>
          <a mat-list-item (click)="setActiveSection('prescriptions')">
            <mat-icon>medication</mat-icon>
            <span>Prescriptions</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <mat-toolbar color="primary">
          <span>Doctor Dashboard</span>
          <span class="spacer"></span>
          <span>Dr. {{ currentUser?.name }}</span>
          <button mat-icon-button (click)="logout()">
            <mat-icon>logout</mat-icon>
          </button>
        </mat-toolbar>

        <div class="content">
          <div [ngSwitch]="activeSection">
            <div *ngSwitchCase="'appointments'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Today's Appointments</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>View and manage your scheduled appointments.</p>
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchCase="'patients'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Patient Records</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Access patient medical records and history.</p>
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchCase="'schedule'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>My Schedule</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Manage your availability and schedule.</p>
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchCase="'prescriptions'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Prescriptions</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Create and manage patient prescriptions.</p>
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchDefault>
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Welcome Dr. {{ currentUser?.name }}</mat-card-title>
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
export class DoctorDashboardComponent {
  activeSection = 'appointments';
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