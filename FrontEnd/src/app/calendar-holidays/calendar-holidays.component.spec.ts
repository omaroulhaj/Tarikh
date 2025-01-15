import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalendarHolidaysComponent } from './calendar-holidays.component';

describe('CalendarHolidaysComponent', () => {
  let component: CalendarHolidaysComponent;
  let fixture: ComponentFixture<CalendarHolidaysComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CalendarHolidaysComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CalendarHolidaysComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
