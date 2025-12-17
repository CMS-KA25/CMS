import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AfterpaymentComp } from './afterpayment-comp';

describe('AfterpaymentComp', () => {
  let component: AfterpaymentComp;
  let fixture: ComponentFixture<AfterpaymentComp>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AfterpaymentComp]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AfterpaymentComp);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
