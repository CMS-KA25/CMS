import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { Patient } from '../Patient.model';
import { PatientService } from '../patient-service';
import { PaymentService, PaymentRequest } from '../payment.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-patientdetails-comp',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule, MatRadioModule, MatButtonModule, MatSnackBarModule, MatDialogModule],
  templateUrl: './patientdetails-comp.html',
  styleUrls: ['./patientdetails-comp.css'],
})
export class PatientdetailsComp {
  patientForm: FormGroup;
  isSubmitting: boolean = false;
  showBillBreakup: boolean = false;
  countryCodes = ['+91', '+1', '+44', '+61', '+33', '+49'];
  maritalStatuses = ['Single', 'Married', 'Divorced', 'Widowed'];
  bloodGroups = ['A+', 'A-', 'B+', 'B-', 'O+', 'O-', 'AB+', 'AB-'];

  maxDate = new Date();

  constructor(private fb: FormBuilder, private service: PatientService, private paymentService: PaymentService, private router: Router, private snackBar: MatSnackBar, private dialog: MatDialog) {
    this.patientForm = this.fb.group({
      date_of_birth: ['', Validators.required],
      sex: ['', Validators.required],
      country: ['', Validators.required],
      pincode: ['', [Validators.required, Validators.pattern(/^[0-9]{6}$/)]],
      city: ['', Validators.required],
      state: ['', Validators.required],
      address: [''],
      marital_status: [''],
      blood_group: ['', Validators.required],
      allergies: [''],
      chief_medical_complaints: [''],
      consulted_before: [false],
      doctor_name: [''],
      last_review_date: [''],
      seeking_followup: [false],
      profile_image_path: [null],
      medical_reports_path: [null]
    });
  }

  onImageSelect(event: any) {
    const file = event.target.files[0];
    if (file && file.type.startsWith('image/')) {
      this.patientForm.patchValue({ profile_image_path: file });
    } else {
      this.snackBar.open('Please select a valid image file', 'Close', { duration: 3000 });
    }
  }

  onPdfSelect(event: any) {
    const file = event.target.files[0];
    if (file && file.type === 'application/pdf') {
      this.patientForm.patchValue({ medical_reports_path: file });
    } else {
      this.snackBar.open('Please select a valid PDF file', 'Close', { duration: 3000 });
    }
  }

  getErrorMessage(fieldName: string): string {
    const field = this.patientForm.get(fieldName);
    if (field?.hasError('required')) {
      return `${fieldName.replace('_', ' ')} is required`;
    }
    if (field?.hasError('email')) {
      return 'Please enter a valid email';
    }
    if (field?.hasError('minlength')) {
      return `${fieldName.replace('_', ' ')} must be at least ${field.errors?.['minlength']?.requiredLength} characters`;
    }
    if (field?.hasError('pattern')) {
      if (fieldName === 'phone_number') return 'Phone number must be 10 digits';
      if (fieldName === 'pincode') return 'Pincode must be 6 digits';
    }
    return '';
  }

  goBack() {
    this.router.navigate(['']);
  }

  ADD() {
    console.log('ADD method called');
    console.log('Form valid:', this.patientForm.valid);
    
    if (this.patientForm.invalid) {
      this.patientForm.markAllAsTouched();
      this.snackBar.open('Please fill all required fields', 'Close', {
        duration: 3000,
        panelClass: ['error-snackbar']
      });
      return;
    }

    this.isSubmitting = true;
    const formValue = this.patientForm.value;
    
    // Format data for API
    const patientData = {
      date_of_birth: formValue.date_of_birth ? new Date(formValue.date_of_birth).toISOString().split('T')[0] : '1990-01-01',
      sex: formValue.sex || 'M',
      country: formValue.country || '',
      pincode: formValue.pincode || '',
      city: formValue.city || '',
      state: formValue.state || '',
      address: formValue.address || '',
      marital_status: formValue.marital_status || '',
      blood_group: formValue.blood_group || '',
      allergies: formValue.allergies || '',
      chief_medical_complaints: formValue.chief_medical_complaints || '',
      consulted_before: formValue.consulted_before || false,
      doctor_name: formValue.doctor_name || '',
      ...(formValue.last_review_date && { last_review_date: new Date(formValue.last_review_date).toISOString().split('T')[0] }),
      seeking_followup: formValue.seeking_followup || false,
      profile_image_path: formValue.profile_image_path?.name || '',
      medical_reports_path: formValue.medical_reports_path?.name || ''
    };

    console.log('Sending patient data:', patientData);

    this.service.addPatient(patientData).subscribe({
      next: (response) => {
        console.log('Success response:', response);
        this.snackBar.open('Form submitted successfully!', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
      },
      error: (error) => {
        console.error('Full error object:', error);
        console.error('Error details:', error.error);
        console.error('Validation errors:', error.error?.errors);
        if (error.error?.errors) {
          Object.keys(error.error.errors).forEach(key => {
            console.error(`${key}:`, error.error.errors[key]);
          });
        }
        this.snackBar.open('Error: ' + (error.error?.title || 'Failed to submit form'), 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.isSubmitting = false;
      },
      complete: () => {
        this.isSubmitting = false;
      }
    });
  }

  getPaymentAmount() {
    const baseAmount = 500;
    const isFollowUp = this.patientForm.get('seeking_followup')?.value;
    return isFollowUp ? baseAmount * 0.7 : baseAmount; // 30% discount for follow-up
  }

  getDiscount() {
    const isFollowUp = this.patientForm.get('seeking_followup')?.value;
    return isFollowUp ? 150 : 0; // 30% of 500
  }

  showBillBreakdown() {
    console.log('showBillBreakdown called');
    this.showBillBreakup = true;
    console.log('showBillBreakup set to:', this.showBillBreakup);
  }

  proceedToPayment() {
    this.showBillBreakup = false;
    this.processPayment();
  }

  cancelPayment() {
    this.showBillBreakup = false;
  }

  makePayment() {
    if (this.patientForm.invalid) {
      this.patientForm.markAllAsTouched();
      this.snackBar.open('Please fill all required fields before payment', 'Close', {
        duration: 3000,
        panelClass: ['error-snackbar']
      });
      return;
    }
    this.showBillBreakdown();
  }

  processPayment() {
    if (this.isSubmitting) return; // Prevent multiple submissions
    this.isSubmitting = true;

    const isFollowup = this.patientForm.get('seeking_followup')?.value || false;
    console.log('Frontend - seeking_followup form value:', this.patientForm.get('seeking_followup')?.value);
    console.log('Frontend - isFollowup variable:', isFollowup);
    
    const paymentRequest: PaymentRequest = {
      amount: this.getPaymentAmount(),
      originalAmount: 500,
      discountAmount: this.getDiscount(),
      isFollowup: isFollowup,
      currency: 'INR',
      patientId: 'temp_patient_id',
      description: 'Consultation Fee'
    };
    
    console.log('Frontend - PaymentRequest object:', paymentRequest);

    this.paymentService.createOrder(paymentRequest).subscribe({
      next: (response) => {
        this.paymentService.initiatePayment(response).then((paymentResponse) => {
          this.paymentService.verifyPayment({
            razorpayOrderId: response.orderId,
            razorpayPaymentId: paymentResponse.razorpay_payment_id,
            razorpaySignature: paymentResponse.razorpay_signature
          }).subscribe({
            next: (verificationResult) => {
              this.snackBar.open('Payment successful!', 'Close', {
                duration: 3000,
                panelClass: ['success-snackbar']
              });
              this.ADD();
              const billPath = verificationResult.billPath || '';
              const queryParams = {
                billPath,
                amount: paymentRequest.amount,
                originalAmount: paymentRequest.originalAmount,
                discountAmount: paymentRequest.discountAmount,
                isFollowup: paymentRequest.isFollowup
              };
              setTimeout(() => this.router.navigate(['/afterpayment'], { queryParams }), 100);
            },
            error: (error) => {
              this.snackBar.open('Payment verification failed', 'Close', {
                duration: 3000,
                panelClass: ['error-snackbar']
              });
              this.isSubmitting = false;
            }
          });
        }).catch((error) => {
          this.snackBar.open('Payment cancelled or failed', 'Close', {
            duration: 3000,
            panelClass: ['error-snackbar']
          });
          this.isSubmitting = false;
        });
      },
      error: (error) => {
        this.snackBar.open('Failed to create payment order', 'Close', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
        this.isSubmitting = false;
      }
    });
  }
}
