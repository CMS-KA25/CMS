import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';
import { RoleType } from './shared/models/auth.models';

export const routes: Routes = [
  { path: '', redirectTo: '/auth/login', pathMatch: 'full' },
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/auth/pages/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'signup',
        loadComponent: () => import('./features/auth/pages/signup/signup.component').then(m => m.SignupComponent)
      },
      {
        path: 'forgot-password',
        loadComponent: () => import('./features/auth/pages/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent)
      },
      {
        path: 'verify-otp',
        loadComponent: () => import('./features/auth/pages/verify-otp/verify-otp.component').then(m => m.VerifyOtpComponent)
      }
      ,
      {
        path: 'accept-invite',
        loadComponent: () => import('./features/admin/accept-invitation.component').then(m => m.AcceptInvitationComponent)
      }
    ]
  },
  {
    path: 'patient',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [RoleType.User] },
    loadComponent: () => import('./features/patient/patient-dashboard.component').then(m => m.PatientDashboardComponent)
  },
  {
    path: 'doctor',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [RoleType.Doctor] },
    loadComponent: () => import('./features/doctor/doctor-dashboard.component').then(m => m.DoctorDashboardComponent)
  },
  {
    path: 'staff',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [RoleType.Staff] },
    loadComponent: () => import('./features/staff/staff-dashboard.component').then(m => m.StaffDashboardComponent)
  },
  {
    path: 'admin',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: [RoleType.Admin] },
    loadComponent: () => import('./features/admin/admin-dashboard.component').then(m => m.AdminDashboardComponent)
  }
];
