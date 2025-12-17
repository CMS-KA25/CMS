import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PatientdetailsComp } from './patientdetails-comp/patientdetails-comp';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,PatientdetailsComp],
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  protected readonly title = signal('project-CMS');
}
