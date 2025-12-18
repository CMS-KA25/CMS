import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../../core/services/auth.service';
import { RoleType } from '../../shared/models/auth.models';

@Component({
  selector: 'bulk-invite',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatSelectModule, MatInputModule],
  template: `
    <div>
      <p>Bulk invite via CSV or Excel. Upload a file with one email per line.</p>
      <div style="display:flex;gap:8px;align-items:center;margin-bottom:8px">
        <input type="file" (change)="onFile($event)" />
        <button mat-stroked-button (click)="downloadTemplate()">Download Template</button>
      </div>
      <mat-form-field>
        <mat-label>Role</mat-label>
        <mat-select [(value)]="role">
          <mat-option [value]="RoleType.Staff">Staff</mat-option>
          <mat-option [value]="RoleType.Doctor">Doctor</mat-option>
        </mat-select>
      </mat-form-field>
      <div style="margin-top:8px">
        <button mat-raised-button color="primary" (click)="upload()" [disabled]="!file || loading">Upload & Invite</button>
        <span *ngIf="loading">Processing...</span>
      </div>
      <div *ngIf="resultList?.length" style="margin-top:8px">
        <div *ngFor="let r of resultList" style="background:#f5f5f5;padding:8px;margin-bottom:6px;display:flex;justify-content:space-between;align-items:center">
          <div style="flex:1">{{ r }}</div>
          <div *ngIf="r.startsWith('AlreadySent:')">
            <button mat-stroked-button (click)="resendFromResult(r)" [disabled]="loading">Resend</button>
          </div>
        </div>
      </div>
    </div>
  `
})
export class BulkInviteComponent {
  RoleType = RoleType;
  file: File | null = null;
  role: RoleType = RoleType.Staff;
  loading = false;
  result: any = null;
  resultList: string[] = [];
  error = '';

  constructor(private auth: AuthService) { }

  onFile(e: Event) {
    const input = e.target as HTMLInputElement;
    this.file = input.files?.[0] ?? null;
  }

  upload() {
    if (!this.file) return;
    this.loading = true;
    this.auth.bulkInvite(this.file, this.role).subscribe({
      next: (res) => {
        this.loading = false;
        this.result = res.data ?? res;
        this.resultList = Array.isArray(this.result) ? this.result.map(String) : [String(this.result)];
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.message || 'Bulk invite failed';
      }
    });
  }

  downloadTemplate() {
    this.loading = true;
    this.auth.downloadBulkInviteTemplate('csv').subscribe({
      next: (blob) => {
        this.loading = false;
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'bulk-invite-template.csv';
        document.body.appendChild(a);
        a.click();
        a.remove();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.message || 'Failed to download template';
      }
    });
  }

  resendFromResult(r: string) {
    const match = r.match(/AlreadySent:\s*([^\s]+)\s*->/);
    const email = match ? match[1] : null;
    if (!email) return;
    this.loading = true;
    this.auth.resendInvite(email, this.role).subscribe({
      next: (res) => {
        this.loading = false;
        // Update the specific entry in resultList
        const idx = this.resultList.indexOf(r);
        if (idx >= 0) this.resultList[idx] = `ReSent: ${email}`;
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.message || 'Resend failed';
      }
    });
  }
}
