import { Routes } from '@angular/router';
import { PatientdetailsComp } from './patientdetails-comp/patientdetails-comp';
import { AfterpaymentComp } from './afterpayment-comp/afterpayment-comp';

export const routes: Routes = [
    { path: 'patients', component: PatientdetailsComp },
    { path: 'afterpayment', component: AfterpaymentComp },

    { path: '', redirectTo: '/patients', pathMatch: 'full' },

      { path: '**', redirectTo: 'patients' }
];
