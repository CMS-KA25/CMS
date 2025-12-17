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
  selector: 'app-staff-dashboard',
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
        <mat-toolbar>Staff Portal</mat-toolbar>
        <mat-nav-list>
          <a mat-list-item (click)="setActiveSection('appointments')">
            <mat-icon>event</mat-icon>
            <span>Manage Appointments</span>
          </a>
          <a mat-list-item (click)="setActiveSection('patients')">
            <mat-icon>people</mat-icon>
            <span>Patient Management</span>
          </a>
          <a mat-list-item (click)="setActiveSection('billing')">
            <mat-icon>receipt</mat-icon>
            <span>Billing</span>
          </a>
          <a mat-list-item (click)="setActiveSection('reports')">
            <mat-icon>assessment</mat-icon>
            <span>Reports</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <mat-toolbar color="primary">
          <span>Staff Dashboard</span>
          <span class="spacer"></span>
          <span>Welcome, {{ currentUser?.name }}</span>
          <button mat-icon-button (click)="logout()">
            <mat-icon>logout</mat-icon>
          </button>
        </mat-toolbar>

        <div class="content">
          <div [ngSwitch]="activeSection">
            <div *ngSwitchCase="'appointments'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Appointment Management</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Schedule and manage patient appointments.</p>
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchCase="'patients'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Patient Management</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Register new patients and update patient information.</p>
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchCase="'billing'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Billing Management</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Process payments and manage billing records.</p>
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchCase="'reports'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Reports</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Generate and view various reports.</p>
                </mat-card-content>
              </mat-card>
            </div>

            <div *ngSwitchDefault>
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Staff Dashboard</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Welcome to the staff portal. Select an option from the sidebar.</p>
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
export class StaffDashboardComponent {
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