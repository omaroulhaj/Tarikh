import { Component } from '@angular/core';

@Component({
  selector: 'app-holiday',
  templateUrl: './holiday.component.html',
  styleUrl: './holiday.component.css'
})
export class HolidayComponent {
  isSidebarClosed = false;
  currentView: 'holiday' | 'calendar' | 'day' = 'calendar';
  currentMonth = new Date();
  selectedDate: Date | null = null;

  constructor() {}

  ngOnInit() {}

  toggleSidebar() {
    this.isSidebarClosed = !this.isSidebarClosed;
  }

  switchView(view: 'calendar' | 'holiday' | 'day'): void {
    this.currentView = view;
  }

  selectDay(date: Date) {
    this.selectedDate = date;
    this.switchView('day'); 
  }
}
