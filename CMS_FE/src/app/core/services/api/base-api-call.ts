import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environment/environment';

@Injectable({ providedIn: 'root' })
export class BaseApiCallServices {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  get<T>(path: string, options?: any) {
    return this.http.get<T>(`${this.baseUrl}/${path}`, options);
  }

  post<T>(path: string, body: unknown, options?: any) {
    return this.http.post<T>(`${this.baseUrl}/${path}`, body, options);
  }
}
