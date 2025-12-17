import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PatientdetailsComp } from './patientdetails-comp';

describe('PatientdetailsComp', () => {
  let component: PatientdetailsComp;
  let fixture: ComponentFixture<PatientdetailsComp>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PatientdetailsComp]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PatientdetailsComp);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
