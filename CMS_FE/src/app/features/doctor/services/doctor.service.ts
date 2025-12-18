import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { ApiResponse } from '../../../shared/models/auth.models';

export interface DoctorProfile {
    specialization: string;
    qualification: string;
    yearOfExperience: number;
    workingDays: string[];
    startTime: string;
    endTime: string;
    slotDuration: number;
    breakStartTime?: string;
    breakEndTime?: string;
}

@Injectable({
    providedIn: 'root'
})
export class DoctorService {
    constructor(private apiService: ApiService) { }

    getProfile(): Observable<ApiResponse<any>> {
        return this.apiService.get<ApiResponse<any>>('doctors/profile');
    }

    updateProfile(profile: DoctorProfile): Observable<ApiResponse<boolean>> {
        return this.apiService.put<ApiResponse<boolean>>('doctors/profile', profile);
    }
}
