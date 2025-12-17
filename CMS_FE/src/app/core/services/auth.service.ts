import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap, catchError, throwError } from 'rxjs';
import { ApiService } from './api.service';
import { User, RoleType, LoginRequest, SignUpRequest, LoginResponse, ApiResponse, VerifyOtpRequest, ForgotPasswordRequest, ResetPasswordRequest, InviteUserRequest } from '../../shared/models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  public loading$;

  constructor(private apiService: ApiService) {
    this.loading$ = this.apiService.loading$;
    this.loadUserFromStorage();
  }

  private loadUserFromStorage(): void {
    const userData = localStorage.getItem('currentUser');
    if (userData) {
      this.currentUserSubject.next(JSON.parse(userData));
    }
  }

  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    this.apiService.setLoading(true);
    return this.apiService.post<ApiResponse<LoginResponse>>('auth/login', credentials)
      .pipe(
        tap(response => {
          if (response.success) {
            this.setCurrentUser(response.data.user);
            localStorage.setItem('accessToken', response.data.accessToken);
            localStorage.setItem('refreshToken', response.data.refreshToken);
          }
          this.apiService.setLoading(false);
        }),
        catchError(error => {
          this.apiService.setLoading(false);
          return throwError(() => error);
        })
      );
  }

  signUp(userData: SignUpRequest): Observable<ApiResponse<any>> {
    this.apiService.setLoading(true);
    // Log payload to help debug client-side payload shape
    console.log('AuthService.signUp payload:', userData);
    return this.apiService.post<ApiResponse<any>>('auth/signup', userData)
      .pipe(
        tap(() => this.apiService.setLoading(false)),
        catchError(error => {
          this.apiService.setLoading(false);
          return throwError(() => error);
        })
      );
  }

  verifyOtp(otpData: VerifyOtpRequest): Observable<ApiResponse<any>> {
    this.apiService.setLoading(true);
    return this.apiService.post<ApiResponse<any>>('auth/verify-signup-otp', otpData)
      .pipe(
        tap(() => this.apiService.setLoading(false)),
        catchError(error => {
          this.apiService.setLoading(false);
          return throwError(() => error);
        })
      );
  }

  forgotPassword(email: ForgotPasswordRequest): Observable<ApiResponse<any>> {
    this.apiService.setLoading(true);
    return this.apiService.post<ApiResponse<any>>('auth/forgot-password', email)
      .pipe(
        tap(() => this.apiService.setLoading(false)),
        catchError(error => {
          this.apiService.setLoading(false);
          return throwError(() => error);
        })
      );
  }

  resetPassword(resetData: ResetPasswordRequest): Observable<ApiResponse<any>> {
    this.apiService.setLoading(true);
    return this.apiService.post<ApiResponse<any>>('auth/reset-password', resetData)
      .pipe(
        tap(() => this.apiService.setLoading(false)),
        catchError(error => {
          this.apiService.setLoading(false);
          return throwError(() => error);
        })
      );
  }

  inviteUser(request: InviteUserRequest): Observable<ApiResponse<any>> {
    this.apiService.setLoading(true);
    return this.apiService.post<ApiResponse<any>>('auth/invite', request).pipe(
      tap(() => this.apiService.setLoading(false)),
      catchError(error => {
        this.apiService.setLoading(false);
        return throwError(() => error);
      })
    );
  }

  bulkInvite(file: File, role: RoleType): Observable<ApiResponse<any>> {
    this.apiService.setLoading(true);
    const fd = new FormData();
    fd.append('file', file);
    fd.append('role', role.toString());
    return this.apiService.postForm<ApiResponse<any>>('auth/bulk-invite', fd).pipe(
      tap(() => this.apiService.setLoading(false)),
      catchError(error => {
        this.apiService.setLoading(false);
        return throwError(() => error);
      })
    );
  }

  downloadBulkInviteTemplate(format = 'csv'): Observable<Blob> {
    return this.apiService.download(`auth/bulk-invite/template?format=${encodeURIComponent(format)}`);
  }

  resendInvite(email: string, role: RoleType): Observable<ApiResponse<any>> {
    this.apiService.setLoading(true);
    return this.apiService.post<ApiResponse<any>>('auth/resend-invite', { email, role }).pipe(
      tap(() => this.apiService.setLoading(false)),
      catchError(error => {
        this.apiService.setLoading(false);
        return throwError(() => error);
      })
    );
  }

  acceptInvite(payload: { token: string; newPassword: string }): Observable<ApiResponse<any>> {
    this.apiService.setLoading(true);
    return this.apiService.post<ApiResponse<any>>('auth/accept-invite', payload).pipe(
      tap(() => this.apiService.setLoading(false)),
      catchError(error => {
        this.apiService.setLoading(false);
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  private setCurrentUser(user: User): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('accessToken') && !!this.getCurrentUser();
  }
}