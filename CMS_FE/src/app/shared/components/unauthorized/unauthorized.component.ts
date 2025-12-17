import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-unauthorized',
    standalone: true,
    imports: [RouterLink, MatButtonModule, MatIconModule],
    template: `
    <div class="unauthorized-container">
      <mat-icon class="huge-icon">lock_person</mat-icon>
      <h1>Unauthorized Access</h1>
      <p>You do not have permission to view this page.</p>
      <div class="actions">
        <button mat-raised-button color="primary" routerLink="/">
          Return to Dashboard
        </button>
      </div>
    </div>
  `,
    styles: [`
    .unauthorized-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 100vh;
      text-align: center;
      padding: 24px;
      color: #1e293b;
    }
    .huge-icon {
      font-size: 96px;
      width: 96px;
      height: 96px;
      margin-bottom: 24px;
      color: #ef4444;
    }
    h1 {
      font-size: 32px;
      font-weight: 800;
      margin-bottom: 16px;
    }
    p {
      font-size: 18px;
      color: #64748b;
      margin-bottom: 32px;
    }
  `]
})
export class UnauthorizedComponent { }
