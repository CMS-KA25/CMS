import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { Router, ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-afterpayment-comp',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatCardModule],
  templateUrl: './afterpayment-comp.html',
  styleUrls: ['./afterpayment-comp.css']
})
export class AfterpaymentComp implements OnInit {
  billPath: string = '';
  paymentDetails: any = {};
  
  constructor(private router: Router, private route: ActivatedRoute, private http: HttpClient) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.billPath = params['billPath'] || '';
      this.paymentDetails = {
        amount: params['amount'] || 0,
        originalAmount: params['originalAmount'] || 0,
        discountAmount: params['discountAmount'] || 0,
        isFollowup: params['isFollowup'] === 'true'
      };
    });
  }

  downloadBill() {
    if (this.billPath) {
      const fileName = this.billPath.split('/').pop();
      const downloadUrl = `http://localhost:5281/api/Payment/download-bill/${fileName}`;
      const link = document.createElement('a');
      link.href = downloadUrl;
      link.download = fileName || 'bill.pdf';
      link.click();
    }
  }

  openPDF() {
    if (this.billPath) {
      const fileName = this.billPath.split('/').pop();
      const viewUrl = `http://localhost:5281/api/Payment/view-bill/${fileName}`;
      window.open(viewUrl, '_blank');
    }
  }

  goHome() {
    this.router.navigate(['/']);
  }
}