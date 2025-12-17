import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeslotButtonComponent } from './timeslot-button.component';

describe('TimeslotButtonComponent', () => {
  let component: TimeslotButtonComponent;
  let fixture: ComponentFixture<TimeslotButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TimeslotButtonComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TimeslotButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
