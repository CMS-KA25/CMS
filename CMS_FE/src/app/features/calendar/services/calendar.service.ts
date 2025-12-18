import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { ApiResponse } from '../../../shared/models/auth.models';

export interface Doctor {
    id: string;
    name: string;
    specialization: string;
    yearOfExperience: number;
    workingDays: number[];
}

export interface TimeSlot {
    id: string;
    doctorId: string;
    startTime: string;
    endTime: string;
    isAvailable: boolean;
    isBooked: boolean;
}

export interface Appointment {
    id?: string;
    doctorId: string;
    patientId: string;
    appointmentDate: string;
    startTime: string;
    endTime: string;
    status: 'Scheduled' | 'Completed' | 'Cancelled' | 'NoShow';
    notes?: string;
}

@Injectable({
    providedIn: 'root'
})
export class CalendarService {
    constructor(private apiService: ApiService) { }

    // Get all active doctors
    getAllDoctors(): Observable<ApiResponse<Doctor[]>> {
        return this.apiService.get<ApiResponse<Doctor[]>>('doctors');
    }

    // Get available time slots for a specific doctor on a specific date
    getAvailableSlots(doctorId: string, date: string, userRole: number = 1): Observable<ApiResponse<any>> {
        return this.apiService.get<ApiResponse<any>>(`timeslots/available?DoctorId=${doctorId}&Date=${date}&UserRole=${userRole}`);
    }

    // Book an appointment
    bookAppointment(appointment: Appointment): Observable<ApiResponse<Appointment>> {
        return this.apiService.post<ApiResponse<Appointment>>('appointments', appointment);
    }

    // Get appointments for a specific doctor
    getDoctorAppointments(doctorId: string, startDate?: string, endDate?: string): Observable<ApiResponse<Appointment[]>> {
        let url = `appointments/doctor/${doctorId}`;
        const params: string[] = [];
        if (startDate) params.push(`startDate=${startDate}`);
        if (endDate) params.push(`endDate=${endDate}`);
        if (params.length > 0) url += `?${params.join('&')}`;

        return this.apiService.get<ApiResponse<Appointment[]>>(url);
    }

    // Get appointments for the current user (patient)
    getMyAppointments(startDate?: string, endDate?: string): Observable<ApiResponse<Appointment[]>> {
        let url = 'appointments/my';
        const params: string[] = [];
        if (startDate) params.push(`startDate=${startDate}`);
        if (endDate) params.push(`endDate=${endDate}`);
        if (params.length > 0) url += `?${params.join('&')}`;

        return this.apiService.get<ApiResponse<Appointment[]>>(url);
    }

    // Cancel an appointment
    cancelAppointment(appointmentId: string, reason?: string): Observable<ApiResponse<any>> {
        return this.apiService.put<ApiResponse<any>>(`appointments/${appointmentId}/cancel`, { reason });
    }

    // Reschedule an appointment
    rescheduleAppointment(appointmentId: string, newDate: string, newStartTime: string): Observable<ApiResponse<Appointment>> {
        return this.apiService.put<ApiResponse<Appointment>>(`appointments/${appointmentId}/reschedule`, {
            newDate,
            newStartTime
        });
    }
}
