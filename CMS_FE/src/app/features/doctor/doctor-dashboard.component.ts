import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { RouterOutlet, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { DoctorProfileComponent } from './components/doctor-profile/doctor-profile.component';

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
    MatDividerModule,
    RouterOutlet,
    DoctorProfileComponent
  ],
  template: `
    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav #drawer class="sidenav" fixedInViewport mode="side" opened>
        <div class="sidebar-header">
          <mat-icon color="primary">medical_services</mat-icon>
          <span>Doctor Portal</span>
        </div>
        
        <mat-nav-list>
          <a mat-list-item (click)="setActiveSection('appointments')" [class.active]="activeSection === 'appointments'">
            <mat-icon matListItemIcon>event</mat-icon>
            <span matListItemTitle>My Appointments</span>
          </a>
          <a mat-list-item (click)="setActiveSection('patients')" [class.active]="activeSection === 'patients'">
            <mat-icon matListItemIcon>people</mat-icon>
            <span matListItemTitle>Patient Records</span>
          </a>
          <a mat-list-item (click)="setActiveSection('schedule')" [class.active]="activeSection === 'schedule'">
            <mat-icon matListItemIcon>schedule</mat-icon>
            <span matListItemTitle>My Schedule</span>
          </a>
          <a mat-list-item (click)="setActiveSection('profile')" [class.active]="activeSection === 'profile'">
            <mat-icon matListItemIcon>person_outline</mat-icon>
            <span matListItemTitle>Profile Setup</span>
          </a>
        </mat-nav-list>

        <div class="sidebar-footer">
          <mat-divider></mat-divider>
          <button mat-button (click)="logout()" class="logout-btn">
            <mat-icon>logout</mat-icon>
            <span>Logout</span>
          </button>
        </div>
      </mat-sidenav>

      <mat-sidenav-content>
        <div class="main-content">
          <div [ngSwitch]="activeSection">
            
            <!-- Appointments -->
            <div *ngSwitchCase="'appointments'">
              <h1 class="page-title">Today's Appointments</h1>
              <mat-card class="info-card">
                 <mat-card-content>
                   <p>You have no appointments scheduled for today.</p>
                 </mat-card-content>
              </mat-card>
            </div>

            <!-- Profile Setup -->
            <div *ngSwitchCase="'profile'">
              <h1 class="page-title">Profile Settings</h1>
              <app-doctor-profile></app-doctor-profile>
            </div>

            <!-- Schedule -->
            <div *ngSwitchCase="'schedule'">
              <h1 class="page-title">My Schedule</h1>
              <mat-card class="info-card">
                <mat-card-content>
                  <p>Manage your availability and working hours in the Profile section.</p>
                  <button mat-stroked-button color="primary" (click)="setActiveSection('profile')">Go to Profile</button>
                </mat-card-content>
              </mat-card>
            </div>

            <!-- Default Welcome -->
            <div *ngSwitchDefault>
              <div class="welcome-section">
                <h1>Welcome, Dr. {{ currentUser?.name }}</h1>
                <p>Manage your clinic operations efficiently from your personal dashboard.</p>
                
                <div class="quick-actions">
                  <mat-card class="action-card" (click)="setActiveSection('profile')">
                    <mat-icon color="primary">account_circle</mat-icon>
                    <h3>Setup Profile</h3>
                    <p>Complete your professional information.</p>
                  </mat-card>
                  
                  <mat-card class="action-card" (click)="setActiveSection('appointments')">
                    <mat-icon color="primary">event</mat-icon>
                    <h3>View Appointments</h3>
                    <p>Check your schedule for the day.</p>
                  </mat-card>
                </div>
              </div>
            </div>

          </div>
        </div>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .sidenav-container {
      height: 100vh;
      background-color: #f4f7f9;
    }
    .sidenav {
      width: 260px;
      border-right: none;
      background: white;
      box-shadow: 2px 0 10px rgba(0,0,0,0.03);
    }
    .sidebar-header {
      padding: 40px 24px;
      display: flex;
      align-items: center;
      gap: 12px;
      font-size: 20px;
      font-weight: 700;
      color: #1e293b;
    }
    .sidebar-footer {
      position: absolute;
      bottom: 0;
      width: 100%;
      padding: 16px;
    }
    .logout-btn {
      width: 100%;
      text-align: left;
      color: #ef4444;
      display: flex;
      align-items: center;
      gap: 8px;
    }
    .main-content {
      padding: 48px;
      max-width: 1100px;
      margin: 0 auto;
    }
    .page-title {
      font-size: 32px;
      font-weight: 800;
      color: #0f172a;
      margin-bottom: 32px;
      letter-spacing: -0.5px;
    }
    .info-card {
      padding: 24px;
      border-radius: 16px;
      box-shadow: 0 4px 20px rgba(0,0,0,0.05) !important;
    }
    .welcome-section {
      text-align: center;
      padding-top: 60px;
    }
    .welcome-section h1 {
      font-size: 48px;
      font-weight: 800;
      color: #0f172a;
    }
    .quick-actions {
      margin-top: 48px;
      display: flex;
      justify-content: center;
      gap: 24px;
    }
    .action-card {
      width: 280px;
      padding: 32px;
      cursor: pointer;
      transition: all 0.3s ease;
      text-align: center;
      border-radius: 16px;
    }
    .action-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 20px 40px rgba(0,0,0,0.08) !important;
    }
    .action-card mat-icon {
      font-size: 40px;
      width: 40px;
      height: 40px;
      margin-bottom: 16px;
    }
    .active {
      color: #1976d2 !important;
      background: #f0f7ff;
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
    // Default to profile if setup might be needed (logic can be expanded)
  }

  setActiveSection(section: string): void {
    this.activeSection = section;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}