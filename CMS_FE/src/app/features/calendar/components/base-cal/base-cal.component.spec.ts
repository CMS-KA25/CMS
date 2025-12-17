import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BaseCalComponent } from './base-cal.component';

describe('BaseCalComponent', () => {
  let component: BaseCalComponent;
  let fixture: ComponentFixture<BaseCalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BaseCalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BaseCalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
