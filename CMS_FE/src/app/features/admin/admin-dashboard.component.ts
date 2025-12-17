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
import { InviteUserComponent } from './invite-user.component';
import { BulkInviteComponent } from './bulk-invite.component';
import { AcceptInvitationComponent } from './accept-invitation.component';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatCardModule,
    RouterOutlet,
    InviteUserComponent,
    BulkInviteComponent,
    AcceptInvitationComponent
  ],
  template: `
    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav #drawer class="sidenav" fixedInViewport mode="side" opened>
        <mat-toolbar>Admin Panel</mat-toolbar>
        <mat-nav-list>
          <a mat-list-item (click)="setActiveSection('invite-staff')">
            <mat-icon>person_add</mat-icon>
            <span>Invite Staff</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <mat-toolbar color="primary">
          <span>Admin Dashboard</span>
          <span class="spacer"></span>
          <span>Welcome, {{ currentUser?.name }}</span>
          <button mat-icon-button (click)="logout()">
            <mat-icon>logout</mat-icon>
          </button>
        </mat-toolbar>

        <div class="content">
          <div [ngSwitch]="activeSection">
            <div *ngSwitchCase="'invite-staff'">
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Invite Staff Members</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Send invitations to new staff members and doctors.</p>
                  <invite-user></invite-user>
                  <bulk-invite></bulk-invite>
                </mat-card-content>
              </mat-card>
            </div>

            

            <div *ngSwitchDefault>
              <mat-card>
                <mat-card-header>
                  <mat-card-title>Admin Dashboard</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                  <p>Welcome to the admin panel. Select an option from the sidebar.</p>
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
export class AdminDashboardComponent {
  activeSection = 'invite-staff';
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