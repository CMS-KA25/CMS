import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'http://localhost:5281/api';
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('accessToken');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` })
    });
  }

  setLoading(loading: boolean): void {
    this.loadingSubject.next(loading);
  }

  get<T>(endpoint: string): Observable<T> {
    return this.http.get<any>(`${this.baseUrl}/${endpoint}`, {
      headers: this.getHeaders()
    }).pipe(
      map(res => this.normalizeResponse<T>(res)),
      catchError(this.handleError.bind(this))
    );
  }

  post<T>(endpoint: string, data: any): Observable<T> {
    return this.http.post<any>(`${this.baseUrl}/${endpoint}`, data, {
      headers: this.getHeaders()
    }).pipe(
      map(res => this.normalizeResponse<T>(res)),
      catchError(this.handleError.bind(this))
    );
  }

  postForm<T>(endpoint: string, formData: FormData): Observable<T> {
    const token = localStorage.getItem('accessToken');
    const headers: { [header: string]: string } = {};
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    return this.http.post<any>(`${this.baseUrl}/${endpoint}`, formData, { headers }).pipe(
      map(res => this.normalizeResponse<T>(res)),
      catchError(this.handleError.bind(this))
    );
  }

  download(endpoint: string): Observable<Blob> {
    const token = localStorage.getItem('accessToken');
    const headers = token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : undefined;
    return this.http.get(`${this.baseUrl}/${endpoint}`, { headers, responseType: 'blob' }).pipe(
      catchError(this.handleError.bind(this))
    );
  }

  put<T>(endpoint: string, data: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}/${endpoint}`, data, {
      headers: this.getHeaders()
    }).pipe(catchError(this.handleError));
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}/${endpoint}`, {
      headers: this.getHeaders()
    }).pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    // Try to extract a useful message from various server shapes
    let message = 'An unknown error occurred';
    if (error.error) {
      if (typeof error.error === 'string') {
        message = error.error;
      } else if (error.error?.message) {
        message = error.error.message;
      } else if (error.error?.errors && Array.isArray(error.error.errors)) {
        message = error.error.errors.join('; ');
      } else if (error.error?.data && error.error.data?.message) {
        message = error.error.data.message;
      }
    } else if (error.message) {
      message = error.message;
    }

    const normalized = {
      status: error.status,
      message,
      error: error.error
    };

    console.error('ApiService HTTP error:', error);
    return throwError(() => normalized);
  }

  private normalizeResponse<T>(res: any): any {
    // Server-side wrapper shape: { Success, Data, Message, ... }
    if (!res) return res;

    // If server already returns the expected shape, return as-is
    if (typeof res.success === 'boolean' && 'data' in res) return res as any;

    // If server uses PascalCase wrapper with inner Data
    if (res.Success && res.Data) {
      return {
        success: res.Data?.success ?? res.Success ?? true,
        data: res.Data?.data ?? res.Data,
        message: res.Data?.message ?? res.Message ?? ''
      } as any;
    }

    // Fallback: wrap raw response as success
    return {
      success: true,
      data: res,
      message: ''
    } as any;
  }
}