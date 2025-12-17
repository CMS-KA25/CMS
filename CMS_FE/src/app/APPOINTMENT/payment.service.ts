import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PaymentRequest {
  amount: number;
  originalAmount: number;
  discountAmount: number;
  isFollowup: boolean;
  currency: string;
  patientId: string;
  description: string;
}

export interface PaymentResponse {
  orderId: string;
  key: string;
  amount: number;
  currency: string;
  description: string;
}

declare var Razorpay: any;

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private apiUrl = 'http://localhost:5281/api/Payment';

  constructor(private http: HttpClient) { }

  createOrder(paymentRequest: PaymentRequest): Observable<PaymentResponse> {
    return this.http.post<PaymentResponse>(`${this.apiUrl}/create-order`, paymentRequest);
  }

  verifyPayment(verificationData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify-payment`, verificationData);
  }

  initiatePayment(paymentData: PaymentResponse): Promise<any> {
    return new Promise((resolve, reject) => {
      const options = {
        key: paymentData.key,
        amount: paymentData.amount * 100,
        currency: paymentData.currency,
        name: 'Clinic Management System',
        description: paymentData.description,
        order_id: paymentData.orderId,
        handler: (response: any) => {
          resolve(response);
        },
        prefill: {
          name: 'Patient',
          email: 'patient@example.com',
          contact: '9999999999'
        },
        theme: {
          color: '#3399cc'
        },
        modal: {
          ondismiss: () => {
            reject('Payment cancelled');
          }
        }
      };

      const rzp = new Razorpay(options);
      rzp.open();
    });
  }
}