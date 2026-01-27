import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TravelExperience } from './travel-experience';

describe('TravelExperience', () => {
  let component: TravelExperience;
  let fixture: ComponentFixture<TravelExperience>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TravelExperience]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TravelExperience);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
