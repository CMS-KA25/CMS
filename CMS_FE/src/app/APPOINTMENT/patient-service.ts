import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Patient } from './Patient.model';

@Injectable({
  providedIn: 'root',
})
export class PatientService {

  private baseurl="http://localhost:5281/api/Patient";

  constructor(private http:HttpClient) { }

  addPatient(patient:Patient):Observable<Patient>
  {
    return this.http.post<Patient>(this.baseurl,patient);
  }
  
}
