import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DoctorSelectorComponent } from './doctor-selector.component';

describe('DoctorSelectorComponent', () => {
  let component: DoctorSelectorComponent;
  let fixture: ComponentFixture<DoctorSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DoctorSelectorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DoctorSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
